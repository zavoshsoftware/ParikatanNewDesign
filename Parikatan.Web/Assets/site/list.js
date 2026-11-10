// product list: category filter + sort (cards fade out, re-order, then fade back in with a stagger)
  (() => {
    const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
    const grid = document.getElementById('plist'); if (!grid) return;
    const cards = [...grid.children], chips = [...document.querySelectorAll('.pb-chips button')];
    const sort = document.getElementById('psort'), count = document.getElementById('pcount');
    const pre = chips.find(b => b.classList.contains('on'));
    let cat = pre ? pre.dataset.cat : 'all', busy = 0;
    const syncUrl = () => { try { history.replaceState(null, '', cat === 'all' ? location.pathname : location.pathname + '?category=' + encodeURIComponent(cat)); } catch (e) {} };
    if (cat !== 'all') {
      let n = 0;
      cards.forEach(c => { const on = c.dataset.cat === cat; c.hidden = !on; if (on) n++; });
      count.textContent = fa(n) + ' محصول';
    }
    const apply = () => {
      const t = ++busy;
      cards.forEach(c => c.classList.add('out'));
      setTimeout(() => {
        if (t !== busy) return;
        const key = sort.value;
        const list = [...cards].sort((a, b) => key === 'low' ? a.dataset.price - b.dataset.price : key === 'high' ? b.dataset.price - a.dataset.price : a.dataset.rec - b.dataset.rec);
        let shown = 0;
        list.forEach(c => {
          grid.appendChild(c);
          const on = cat === 'all' || c.dataset.cat === cat;
          c.hidden = !on;
          if (on) { c.style.transitionDelay = (shown++ * 60) + 'ms'; void c.offsetWidth; c.classList.remove('out'); }   // reflow so the fade-in transition runs
        });
        count.textContent = fa(shown) + ' محصول';
        setTimeout(() => cards.forEach(c => c.style.transitionDelay = ''), 700);
      }, 260);
    };
    chips.forEach(b => b.addEventListener('click', () => {
      if (b.dataset.cat === cat) return;
      cat = b.dataset.cat; chips.forEach(x => x.classList.toggle('on', x === b)); syncUrl(); apply();
    }));
    sort.addEventListener('change', apply);

    // filter bar gets a hairline + shadow once it sticks
    const bar = document.getElementById('plpbar');
    const stuck = () => bar.classList.toggle('stuck', bar.getBoundingClientRect().top <= 0 && scrollY > 0);
    addEventListener('scroll', stuck, { passive: true }); stuck();
  })();