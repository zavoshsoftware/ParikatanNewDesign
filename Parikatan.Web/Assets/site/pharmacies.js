// pharmacies: search (name or city), city filter, "24-hour" / "store" filter, city sidebar with scroll-spy
  (() => {
    const $ = id => document.getElementById(id);
    const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
    const norm = s => s.replace(/[يى]/g, 'ی').replace(/ك/g, 'ک').replace(/[‌\-–_]/g, ' ').replace(/\s+/g, ' ').trim();
    const lite = s => s.replace(/[يى]/g, 'ی').replace(/ك/g, 'ک');   // same length as the original, for highlighting
    const regs = [...document.querySelectorAll('.ph-reg')], side = [...document.querySelectorAll('#phside a')];
    const q = $('phq'), city = $('phcity'), count = $('phcount'), empty = $('phempty');
    const kinds = [...document.querySelectorAll('.ph-kinds button')];
    let kind = 'all';
    const items = regs.flatMap(r => [...r.querySelectorAll('.ph-item')].map(li => {
      const b = li.querySelector('b');
      return { li, b, name: b.textContent, key: norm(b.textContent + ' ' + r.dataset.region), reg: r, h24: !!li.querySelector('.bd.h24'), store: li.dataset.kind === 'store' };
    }));

    const apply = () => {
      const term = norm(q.value), only = city.value;
      let total = 0;
      regs.forEach(r => r._n = 0);
      items.forEach(it => {
        const ok = (!only || it.reg.id === only) && (kind === 'all' || (kind === 'h24' ? it.h24 : it.store)) && (!term || it.key.includes(term));
        it.li.hidden = !ok;
        if (ok) { it.reg._n++; total++; }
        // highlight the matched part of the name
        if (ok && term) {
          const t = lite(q.value.trim()), i = t ? lite(it.name).indexOf(t) : -1;
          it.b.innerHTML = i < 0 ? it.name : it.name.slice(0, i) + '<mark>' + it.name.slice(i, i + t.length) + '</mark>' + it.name.slice(i + t.length);
        } else if (it.b.firstElementChild) it.b.textContent = it.name;
      });
      regs.forEach(r => { r.hidden = !r._n; r.querySelector('.cnt').textContent = fa(r._n) + ' فروشگاه'; });
      side.forEach(a => { const r = $(a.dataset.r); a.classList.toggle('off', !r._n); a.querySelector('i').textContent = fa(r._n); });
      count.textContent = fa(total) + ' مرکز فروش';
      empty.hidden = !!total;
    };
    q.addEventListener('input', apply);
    city.addEventListener('change', () => { apply(); if (city.value) scrollTo({ top: $('phlist').getBoundingClientRect().top + scrollY - 90, behavior: 'smooth' }); });
    kinds.forEach(b => b.addEventListener('click', () => { kind = b.dataset.k; kinds.forEach(x => x.classList.toggle('on', x === b)); apply(); }));
    apply();

    // sidebar: highlight the city in view; sticky bar gets a hairline once it sticks
    const bar = $('phbar');
    let raf = 0;
    const spy = () => {
      raf = 0;
      bar.classList.toggle('stuck', bar.getBoundingClientRect().top <= 0 && scrollY > 0);
      let cur = null;
      regs.forEach(r => { if (!r.hidden && r.getBoundingClientRect().top < 160) cur = r; });
      side.forEach(a => a.classList.toggle('on', !!cur && a.dataset.r === cur.id));
    };
    addEventListener('scroll', () => { if (!raf) raf = requestAnimationFrame(spy); }, { passive: true }); spy();
  })();