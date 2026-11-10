// magazine: list filter/search and article reading aids
(() => {
  const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);

  // ---------- list: topic chips + search (featured card included) ----------
  const chipWrap = document.querySelector('.bl-chips');
  const chips = chipWrap ? [...chipWrap.querySelectorAll('button')] : [];
  const q = document.getElementById('bsearch');
  if (q) {
    const cards = [...document.querySelectorAll('.bl-list .ac')], empty = document.getElementById('bempty');
    const norm = s => s.replace(/[يى]/g, 'ی').replace(/ك/g, 'ک').replace(/‌/g, ' ').trim();
    let cat = 'all', t = 0;
    const apply = () => {
      const term = norm(q.value);
      cards.forEach(c => c.classList.add('out'));
      clearTimeout(t);
      t = setTimeout(() => {
        let shown = 0;
        cards.forEach(c => {
          const on = (cat === 'all' || c.dataset.cat === cat) && (!term || norm(c.textContent).includes(term));
          c.hidden = !on;
          if (on) { c.style.transitionDelay = (shown++ * 50) + 'ms'; void c.offsetWidth; c.classList.remove('out'); }
        });
        if (empty) empty.hidden = !!shown;
        setTimeout(() => cards.forEach(c => c.style.transitionDelay = ''), 600);
      }, 220);
    };
    chips.forEach(b => b.addEventListener('click', () => {
      if (b.dataset.cat === cat) return;
      cat = b.dataset.cat; chips.forEach(x => x.classList.toggle('on', x === b)); apply();
    }));
    q.addEventListener('input', apply);
  }

  // ---------- article: reading progress, active table-of-contents item, copy link ----------
  const bar = document.getElementById('rprog');
  if (bar) {
    const prose = document.querySelector('.prose'), links = [...document.querySelectorAll('.toc a')];
    const heads = links.map(a => document.querySelector(a.getAttribute('href'))).filter(Boolean);
    let raf = 0;
    const paint = () => {
      raf = 0;
      const r = prose.getBoundingClientRect(), total = r.height - innerHeight * 0.6;
      bar.style.transform = `scaleX(${Math.min(1, Math.max(0, -r.top / (total || 1))).toFixed(3)})`;
      let cur = heads[0];
      heads.forEach(h => { if (h.getBoundingClientRect().top < innerHeight * 0.35) cur = h; });
      links.forEach(a => a.classList.toggle('on', a.getAttribute('href') === '#' + (cur && cur.id)));
    };
    addEventListener('scroll', () => { if (!raf) raf = requestAnimationFrame(paint); }, { passive: true });
    addEventListener('resize', paint); paint();
    const copy = document.getElementById('copylink');
    if (copy) copy.addEventListener('click', () => {
      const say = msg => window.PK && PK.toast(msg, false), fail = () => say('کپی لینک ممکن نشد؛ آدرس صفحه را از نوار مرورگر کپی کنید');
      if (navigator.clipboard) navigator.clipboard.writeText(location.href.split('#')[0]).then(() => say('لینک مقاله کپی شد'), fail); else fail();
    });
  }

  // ---------- article: comment form ----------
  const cf = document.getElementById('cm-form');
  if (cf) cf.addEventListener('submit', e => {
    e.preventDefault();
    const g = id => document.getElementById(id).value.trim(), msg = document.getElementById('cm-msg');
    msg.className = 'cm-msg';
    if (!g('cm-name') || !g('cm-email') || !g('cm-body')) { msg.textContent = 'نام، ایمیل و متن نظر را وارد کنید.'; msg.classList.add('err'); return; }
    fetch('/BlogComments/SubmitComment', {
      method: 'POST', credentials: 'same-origin',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8', 'X-Requested-With': 'XMLHttpRequest' },
      body: new URLSearchParams({ name: g('cm-name'), email: g('cm-email'), body: g('cm-body'), code: cf.dataset.code, site: '' }).toString()
    }).then(r => r.json()).then(res => {
      if (String(res) === 'true') { msg.textContent = 'نظر شما ثبت شد و پس از تأیید نمایش داده می‌شود.'; msg.classList.add('ok'); cf.reset(); }
      else if (String(res) === 'InvalidEmail') { msg.textContent = 'ایمیل واردشده درست نیست.'; msg.classList.add('err'); }
      else { msg.textContent = 'ثبت نظر انجام نشد.'; msg.classList.add('err'); }
    }).catch(() => { msg.textContent = 'ارتباط با سرور برقرار نشد.'; msg.classList.add('err'); });
  });
})();