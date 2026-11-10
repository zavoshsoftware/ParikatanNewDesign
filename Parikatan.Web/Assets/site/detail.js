// product page: gallery, quantity, add to cart, sticky tabs, mobile buy bar, review modal
(() => {
  const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);

  // gallery: thumbnails swap the main image with a short cross-fade
  const gimg = document.getElementById('gimg');
  document.querySelectorAll('.gt[data-src]').forEach(t => t.addEventListener('click', () => {
    if (!gimg || t.classList.contains('on')) return;
    document.querySelectorAll('.gt').forEach(x => x.classList.toggle('on', x === t));
    gimg.classList.add('swap');
    setTimeout(() => {
      gimg.src = t.dataset.src; gimg.alt = t.dataset.alt || '';
      gimg.classList.toggle('photo', t.hasAttribute('data-photo'));
      requestAnimationFrame(() => gimg.classList.remove('swap'));
    }, 200);
  }));

  // quantity stepper (capped by stock, max 10)
  let q = 1; const qv = document.getElementById('qv');
  const max = qv ? Math.max(1, +qv.dataset.max || 10) : 10;
  document.querySelectorAll('[data-q]').forEach(b => b.addEventListener('click', () => {
    q = Math.min(max, Math.max(1, q + Number(b.dataset.q))); qv.textContent = fa(q);
  }));

  // add to cart (main button and mobile bar)
  [document.getElementById('addbtn'), document.getElementById('addbtn2')].forEach(btn => {
    if (!btn) return;
    btn.addEventListener('click', e => {
      e.preventDefault();
      if (window.PK) PK.add(btn.dataset.id, q, btn.dataset.name || '');
    });
  });

  // sticky tabs: highlight the section in view
  const links = [...document.querySelectorAll('.tabs a')];
  const secs = links.map(a => document.querySelector(a.getAttribute('href'))).filter(Boolean);
  if (secs.length) {
    const spy = () => {
      let cur = secs[0];
      secs.forEach(s => { if (s.getBoundingClientRect().top <= 120) cur = s; });
      links.forEach(a => a.classList.toggle('on', a.getAttribute('href') === '#' + cur.id));
    };
    addEventListener('scroll', spy, { passive: true }); spy();
  }

  // mobile buy bar: appears once the price box has scrolled out of view
  const bar = document.getElementById('sbar'), box = document.getElementById('pricebox');
  if (bar && box) {
    const upd = () => {
      const past = box.getBoundingClientRect().bottom < 0;
      bar.classList.toggle('show', past); bar.setAttribute('aria-hidden', String(!past));
    };
    addEventListener('scroll', upd, { passive: true }); upd();
  }
})();

// review modal: validation, POST to the product's comment endpoint, success state
(() => {
  const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
  const modal = document.getElementById('rmodal'); if (!modal) return;
  const formView = document.getElementById('rm-form-view'), doneView = document.getElementById('rm-done-view');
  const form = document.getElementById('rm-form');
  const name = document.getElementById('rm-name'), text = document.getElementById('rm-text'), cnt = document.getElementById('rm-cnt');
  const errName = document.getElementById('err-name'), errText = document.getElementById('err-text');
  let opener = null;

  text.addEventListener('input', () => {
    cnt.textContent = fa(text.value.length) + ' / ۵۰۰';
    if (text.value.trim().length >= 10) { errText.textContent = ''; text.classList.remove('bad'); }
  });
  name.addEventListener('input', () => { if (name.value.trim()) { errName.textContent = ''; name.classList.remove('bad'); } });

  const open = () => {
    opener = document.activeElement;
    modal.classList.add('open'); modal.setAttribute('aria-hidden', 'false'); document.body.classList.add('rm-open');
    formView.hidden = false; doneView.hidden = true;
    setTimeout(() => modal.querySelector('.rm-dlg').focus({ preventScroll: true }), 30);
  };
  const close = () => {
    modal.classList.remove('open'); modal.setAttribute('aria-hidden', 'true'); document.body.classList.remove('rm-open');
    if (opener && opener.focus) opener.focus({ preventScroll: true });
  };
  document.querySelectorAll('[data-rm-open]').forEach(b => b.addEventListener('click', open));
  modal.querySelectorAll('[data-rm-close]').forEach(b => b.addEventListener('click', close));
  document.addEventListener('keydown', e => { if (e.key === 'Escape' && modal.classList.contains('open')) close(); });

  form.addEventListener('submit', e => {
    e.preventDefault();
    let first = null;
    errName.textContent = errText.textContent = '';
    name.classList.remove('bad'); text.classList.remove('bad');
    if (!name.value.trim()) { errName.textContent = 'لطفاً نام خود را وارد کنید.'; name.classList.add('bad'); first = first || name; }
    if (text.value.trim().length < 10) { errText.textContent = 'متن نظر باید دست‌کم ۱۰ نویسه باشد.'; text.classList.add('bad'); first = first || text; }
    if (first) { first.focus(); return; }

    const btn = form.querySelector('button[type=submit]'); btn.disabled = true;
    fetch(form.dataset.url, {
      method: 'POST', credentials: 'same-origin',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8', 'X-Requested-With': 'XMLHttpRequest' },
      body: new URLSearchParams({ name: name.value.trim(), message: text.value.trim() }).toString()
    }).then(r => r.json()).then(res => {
      if (res.ok) {
        formView.hidden = true; doneView.hidden = false;
        form.reset(); cnt.textContent = '۰ / ۵۰۰';
      } else {
        errText.textContent = res.message || 'ثبت نظر انجام نشد.';
      }
    }).catch(() => { errText.textContent = 'خطایی رخ داد؛ دوباره تلاش کنید.'; })
      .finally(() => { btn.disabled = false; });
  });
})();
