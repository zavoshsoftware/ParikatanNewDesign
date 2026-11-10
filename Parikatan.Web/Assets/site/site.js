/* Parikatan site script: cart is server-backed (cookie "basket-parikatan" = "id^qty/id^qty/", written by POST /Basket/AddToCart). */
window.PK = (() => {
  const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
  const readCart = () => {
    const m = document.cookie.match(/(?:^|;\s*)basket-parikatan=([^;]*)/);
    const out = {};
    if (!m) return out;
    decodeURIComponent(m[1]).split('/').forEach(p => { const a = p.split('^'); if (a[0] && +a[1] > 0) out[a[0]] = +a[1]; });
    return out;
  };
  const count = () => Object.values(readCart()).reduce((s, n) => s + n, 0);
  const badge = bump => document.querySelectorAll('[data-cart-count]').forEach(b => {
    const n = count(); b.textContent = fa(n); b.hidden = !n;
    if (bump) { b.classList.remove('bump'); void b.offsetWidth; b.classList.add('bump'); }
  });
  let tt = 0;
  const toast = (msg, link = true, ok = true) => {
    let t = document.getElementById('pk-toast');
    if (!t) { t = document.createElement('div'); t.id = 'pk-toast'; t.className = 'pk-toast'; t.setAttribute('role', 'status'); document.body.appendChild(t); }
    t.innerHTML = `<span class="tk-ic"><svg class="i"><use href="${ok ? '#check' : '#x'}"/></svg></span><span></span>` + (link ? '<a href="/basket">مشاهده سبد</a>' : '');
    t.children[1].textContent = msg;
    t.classList.add('show'); clearTimeout(tt); tt = setTimeout(() => t.classList.remove('show'), 3200);
  };
  const post = (url, data) => fetch(url, {
    method: 'POST', credentials: 'same-origin',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8', 'X-Requested-With': 'XMLHttpRequest' },
    body: new URLSearchParams(data).toString()
  }).then(r => r.json());
  const add = (id, qty = 1, name = '') => {
    const next = Math.min(10, (readCart()[id] || 0) + qty);
    return post('/Basket/AddToCart', { code: id, qty: next }).then(res => {
      if (res === 'true') { badge(true); toast(name ? `«${name}» به سبد خرید اضافه شد` : 'محصول به سبد خرید اضافه شد'); }
      else toast('این محصول در حال حاضر موجود نیست.', false, false);
    }).catch(() => toast('خطایی رخ داد؛ دوباره تلاش کنید.', false, false));
  };
  // every "add to cart" control carries data-add="<product id>" (+ optional data-name)
  document.addEventListener('click', e => {
    const b = e.target.closest('[data-add]'); if (!b || !b.dataset.add) return;
    e.preventDefault();
    add(b.dataset.add, 1, b.dataset.name || '');
  });
  // newsletter + dealership forms post to Home actions
  document.addEventListener('submit', e => {
    const f = e.target.closest('form[data-post]'); if (!f) return;
    e.preventDefault();
    const btn = f.querySelector('button[type=submit], button.btn'); if (btn) btn.disabled = true;
    post(f.dataset.post, Object.fromEntries(new FormData(f))).then(res => {
      toast(res.message || 'ثبت شد', false, !!res.ok);
      if (res.ok) f.reset();
    }).catch(() => toast('خطایی رخ داد؛ دوباره تلاش کنید.', false, false)).finally(() => { if (btn) btn.disabled = false; });
  });
  badge();
  return { get: readCart, add, count, fa, toast };
})();
  // hero: scroll progress through the pinned stage drives the slogans, wordmark, jar lid and product
  (() => {
    const hero = document.getElementById('hero'); if (!hero) return;
    if (window.matchMedia && matchMedia('(prefers-reduced-motion: reduce)').matches) return;
    const stage = hero.querySelector('.hstage');
    const clamp = (v, a = 0, b = 1) => Math.min(b, Math.max(a, v));
    const ease = x => 1 - Math.pow(1 - x, 2.2);
    const inOut = x => x < .5 ? 2 * x * x : 1 - Math.pow(-2 * x + 2, 2) / 2;
    let raf = 0;
    const paint = () => {
      raf = 0;
      const span = hero.offsetHeight - stage.offsetHeight;
      const pv = location.hash.match(/p=([\d.]+)/);   // #p=0.5 freezes progress (used for previews)
      const p = pv ? clamp(+pv[1]) : span > 0 ? clamp(-hero.getBoundingClientRect().top / span) : 0;
      const L = clamp((p - 0.06) / 0.36), le = inOut(L);                 // lid phase
      const v = {
        t: ease(clamp(p / 0.45)), ta: 1 - clamp(p / 0.34),               // slogans travel outward and fade
        f: clamp((p - 0.12) / 0.5),                                      // wordmark fills with ink
        // lid: lifts in an arc, tilts and comes to rest leaning on the jar (where the open-jar photo has it)
        lx: 34.7 * le + '%', ly: 11.6 * le - 34 * Math.sin(Math.PI * L) + '%', lr: 56 * le + 'deg', ls: 1 - 0.04 * le,
        lift: Math.sin(Math.PI * L), le: le,                             // le recentres the jar as the lid moves to its side
        oo: clamp((L - 0.82) / 0.18),                                    // swap to the open-jar photo as it lands
        z: ease(clamp((p - 0.46) / 0.54)),                               // product comes forward
        o: 1 - clamp((p - 0.74) / 0.22)                                  // …and is removed
      };
      for (const k in v) hero.style.setProperty('--' + k, typeof v[k] === 'number' ? v[k].toFixed(4) : v[k]);
    };
    const req = () => { if (!raf) raf = requestAnimationFrame(paint); };
    addEventListener('scroll', req, { passive: true }); addEventListener('resize', req); paint();
  })();
  // sections: scroll-linked motion below the hero
  //  · photos drift inside their frames (parallax)   · tile columns move at different speeds
  //  · framed blocks (routine, dealership) grow into place   · headings rise word by word   · stats count up
  (() => {
    if (/static/.test(location.search + location.hash) || (window.matchMedia && matchMedia('(prefers-reduced-motion: reduce)').matches)) return;
    const $$ = s => [...document.querySelectorAll(s)];
    const clamp = (v, a = 0, b = 1) => Math.min(b, Math.max(a, v));
    const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);

    // headings: wrap each word so it can rise out of a mask when the heading enters the viewport
    const heads = $$('main h2').filter(h => !h.closest('.prose'));
    heads.forEach(h => {
      if (h.children.length) return;   // keep headings with inline markup as they are
      const words = h.textContent.trim().split(/\s+/);
      h.innerHTML = words.map((w, i) => `<span class="w"><span style="--i:${i}">${w}</span></span>`).join(' ');
      h.classList.add('wr');
    });
    // stats: numbers written in Persian digits count up from zero
    const counters = $$('.sb-item b').filter(b => /^[۰-۹]+$/.test(b.textContent.trim()));
    counters.forEach(b => { b.dataset.to = b.textContent.trim().replace(/[۰-۹]/g, d => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d)); b.textContent = '۰'; });
    const count = b => {
      const to = +b.dataset.to, t0 = performance.now();
      const step = t => { const k = clamp((t - t0) / 1400); b.textContent = fa(Math.round(to * (1 - Math.pow(1 - k, 3)))); if (k < 1) requestAnimationFrame(step); };
      requestAnimationFrame(step);
    };
    const io = new IntersectionObserver(es => es.forEach(e => {
      if (!e.isIntersecting) return; io.unobserve(e.target);
      if (e.target.tagName === 'H2') e.target.classList.add('on'); else count(e.target);
    }), { rootMargin: '0px 0px -12% 0px' });
    heads.filter(h => h.classList.contains('wr')).forEach(h => io.observe(h)); counters.forEach(b => io.observe(b));

    // scroll-linked: [data-px] drifts inside its frame, [data-py] moves against the scroll, [data-grow] gets --g
    $$('.pc .img img, .rt-vis img, .tile img').forEach(el => el.dataset.px = 7);
    const tiles = $$('.tile-grid .tile'); if (tiles[1]) { tiles[0].dataset.py = 30; tiles[1].dataset.py = -40; if (tiles[2]) tiles[2].dataset.py = 30; }
    $$('.rt-row, .dbox, .res, .vm-card, .ct-sms').forEach(el => el.dataset.grow = '');
    const px = $$('[data-px]'), py = $$('[data-py]'), gr = $$('[data-grow]');
    let raf = 0;
    const paint = () => {
      raf = 0;
      const vh = innerHeight, wide = innerWidth > 760;
      const q = r => clamp((r.top + r.height / 2 - vh / 2) / (vh / 2 + r.height / 2), -1, 1);   // -1 above … 0 centred … 1 below
      px.forEach(el => { const r = el.parentElement.getBoundingClientRect(); if (r.bottom < -50 || r.top > vh + 50) return; el.style.translate = `0 ${(q(r) * el.dataset.px).toFixed(2)}%`; });
      py.forEach(el => { const r = el.getBoundingClientRect(); el.style.translate = wide ? `0 ${(q(r) * el.dataset.py).toFixed(1)}px` : ''; });
      gr.forEach(el => { const r = el.getBoundingClientRect(); el.style.setProperty('--g', clamp((vh - r.top) / (vh * 0.7)).toFixed(3)); });
    };
    const req = () => { if (!raf) raf = requestAnimationFrame(paint); };
    addEventListener('scroll', req, { passive: true }); addEventListener('resize', req); paint();
  })();
  // scroll reveal: elements fade in and rise 18px the first time they enter the viewport
  // (70px inside the edge), once only; groups (cards, grids) are staggered.
  // Skipped for reduced-motion, no IntersectionObserver, or when the URL contains "static" (previews).
  (() => {
    const off = !('IntersectionObserver' in window) || /static/.test(location.search + location.hash) ||
      (window.matchMedia && matchMedia('(prefers-reduced-motion: reduce)').matches);
    if (off) return;
    const groups = [];
    const add = (trigger, items) => { items = items.filter(Boolean); if (trigger && items.length) groups.push({ trigger, items }); };
    const all = (sel, root = document) => [...root.querySelectorAll(sel)];
    const cs = document.querySelector('.catstrip'); if (cs) add(cs, all('.cs-item', cs));
    all('main .sh').forEach(el => add(el, [el]));
    all('main .car').forEach(car => add(car, [...(car.querySelector('[data-track]') || car).children]));
    const rt = document.querySelector('.rt-row'); if (rt) add(rt, [...rt.children]);
    ['.sb-grid', '.tile-grid', '.agrid', '.dbox', '.bgrid', '.pgrid', '.tgrid', '.usteps', '.rcards', '.stats', '.why-grid', '.ph-text', '.buy', '.specs', '.ab-values', '.ab-focus'].forEach(sel => all(sel).forEach(g => add(g, [...g.children])));
    document.documentElement.classList.add('rv-on');
    const io = new IntersectionObserver(entries => entries.forEach(en => {
      if (!en.isIntersecting) return;
      io.unobserve(en.target);
      const g = en.target._rv; if (!g) return;
      g.items.forEach((el, i) => {
        el.style.setProperty('--rv-d', Math.min(i, 6) * 0.07 + 's');
        el.classList.add('rv-in');
        // hand control back to the element's own transform / hover styles once the reveal has played
        setTimeout(() => { el.removeAttribute('data-rv'); el.classList.remove('rv-in'); el.style.removeProperty('--rv-d'); }, 600 + Math.min(i, 6) * 70 + 80);
      });
    }), { rootMargin: '0px 0px -70px 0px', threshold: 0 });
    groups.forEach(g => { g.items.forEach(el => el.setAttribute('data-rv', '')); g.trigger._rv = g; io.observe(g.trigger); });
  })();
  // mobile menu drawer (+ products accordion)
  (() => {
    const btn = document.querySelector('.menu-btn'), drawer = document.getElementById('mdrawer'), acc = document.getElementById('macc');
    if (!btn || !drawer) return;
    const open = () => { document.body.classList.add('menu-open'); btn.setAttribute('aria-expanded', 'true'); };
    const close = () => { document.body.classList.remove('menu-open'); btn.setAttribute('aria-expanded', 'false'); };
    btn.setAttribute('aria-expanded', 'false'); btn.setAttribute('aria-controls', 'mdrawer');
    btn.addEventListener('click', open);
    document.querySelectorAll('[data-close]').forEach(el => el.addEventListener('click', close));
    document.addEventListener('keydown', e => { if (e.key === 'Escape') close(); });
    const trig = acc.querySelector('.trig');
    const setAcc = on => { acc.classList.toggle('open', on); trig.setAttribute('aria-expanded', String(on)); };
    trig.addEventListener('click', () => setAcc(!acc.classList.contains('open')));
    const m = location.hash.match(/^#mobile-menu(?:=(products))?/);   // used for previews
    if (m) { open(); if (m[1]) setAcc(true); }
  })();
  // products mega menu: preview card follows the hovered / focused item
  (() => {
    const mega = document.querySelector('.has-mega'); if (!mega) return;
    const items = [...mega.querySelectorAll('.mi[data-img]')]; if (!items.length) return;
    const featBtn = mega.querySelector('.mega-feat .btn');
    const img = mega.querySelector('.feat-img'), title = mega.querySelector('.feat-title'), tag = mega.querySelector('.feat-tag'), price = mega.querySelector('.feat-price');
    const show = it => {
      items.forEach(x => x.classList.toggle('on', x === it));
      img.src = it.dataset.img; img.alt = it.dataset.title; title.textContent = it.dataset.title;
      tag.textContent = it.dataset.tag; price.textContent = it.dataset.price;
      if (featBtn) { featBtn.dataset.add = it.dataset.id; featBtn.dataset.name = it.dataset.title; }
    };
    items.forEach(it => { it.addEventListener('mouseenter', () => show(it)); it.addEventListener('focus', () => show(it)); });
    show(items[0]);
    const m = location.hash.match(/^#menu(?:=(\d))?/);   // #menu or #menu=3 opens the panel (used for previews)
    if (m) { mega.classList.add('open'); if (m[1]) show(items[+m[1] - 1]); }
  })();
  // generic horizontal carousel: shared by the best-sellers row and the ingredients row.
  // Uses scrollBy on the RTL track; modern browsers report scrollLeft as 0 at the first
  // (rightmost) item down to -maxScroll at the last (leftmost) item, so "next" is negative.
  document.querySelectorAll('[data-car]').forEach(car => {
    const track = car.querySelector('[data-track]');
    car.querySelectorAll('.car-btn').forEach(btn => {
      btn.addEventListener('click', () => {
        const card = track.querySelector(':scope > *');
        const amount = (card ? card.getBoundingClientRect().width : 280) + 28;
        const dir = btn.classList.contains('next') ? -1 : 1;
        track.scrollBy({ left: dir * amount, behavior: 'smooth' });
      });
    });
  });
  // magazine carousel (mobile only): keep the pager dots in sync with scroll position
  document.querySelectorAll('.agrid').forEach(track => {
    const dots = track.nextElementSibling && track.nextElementSibling.querySelectorAll('i');
    if (!dots || !dots.length) return;
    track.addEventListener('scroll', () => {
      const edge = track.getBoundingClientRect().right;
      let best = 0, bestD = Infinity;
      [...track.children].forEach((c, i) => { const d = Math.abs(c.getBoundingClientRect().right - edge); if (d < bestD) { bestD = d; best = i; } });
      dots.forEach((d, i) => d.classList.toggle('on', i === best));
    }, { passive: true });
  });
