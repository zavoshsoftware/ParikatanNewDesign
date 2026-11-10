// cart page: server-backed quantity / remove / coupon, summary refresh
(() => {
  const PK = window.PK;
  const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
  const money = n => fa(Math.round(n).toLocaleString('en-US').replace(/,/g, '٬')) + ' تومان';
  const $ = id => document.getElementById(id);
  const post = (url, data) => fetch(url, {
    method: 'POST', credentials: 'same-origin',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8', 'X-Requested-With': 'XMLHttpRequest' },
    body: new URLSearchParams(data).toString()
  }).then(r => r.json());

  const refreshBadge = () => document.querySelectorAll('[data-cart-count]').forEach(b => {
    const m = document.cookie.match(/(?:^|;\s*)basket-parikatan=([^;]*)/); let n = 0;
    if (m) decodeURIComponent(m[1]).split('/').forEach(p => { const a = p.split('^'); if (a[0] && +a[1] > 0) n += +a[1]; });
    b.textContent = fa(n); b.hidden = !n;
  });

  const apply = t => {
    $('ccount').textContent = t.count ? fa(t.count) + ' کالا' : '';
    $('sqty').textContent = fa(t.count);
    $('ssub').textContent = money(t.subTotal);
    $('sdisc-row').hidden = !t.discount;
    $('sdisc').textContent = '−' + money(t.discount);
    $('sship').textContent = t.shipping > 0 ? money(t.shipping) : 'رایگان';
    $('stotal').textContent = money(t.total);
    (t.rows || []).forEach(r => {
      const row = document.querySelector('.ci[data-id="' + r.id + '"]'); if (!row) return;
      row.querySelector('.qty span').textContent = fa(r.qty);
      if (r.amount !== undefined) row.querySelector('.ci-total').textContent = money(r.amount);
      const max = +row.dataset.max || 10;
      row.querySelector('[data-d="-1"]').disabled = r.qty <= 1;
      row.querySelector('[data-d="1"]').disabled = r.qty >= max;
    });
    if (!t.count) { $('cart-full').hidden = true; $('cart-empty').hidden = false; }
    refreshBadge();
  };

  $('citems').addEventListener('click', e => {
    const row = e.target.closest('.ci'); if (!row) return;
    const id = row.dataset.id;
    const step = e.target.closest('[data-d]');
    if (step) {
      if (step.disabled) return;
      const cur = +row.querySelector('.qty span').textContent.replace(/[۰-۹]/g, d => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d));
      const max = +row.dataset.max || 10;
      const next = Math.min(max, Math.max(1, cur + +step.dataset.d));
      row.querySelectorAll('.qty button').forEach(b => b.disabled = true);
      post('/Basket/SetQuantity', { code: id, qty: next }).then(t => { if (t.ok) { apply(t); $('cmsg').textContent = ''; } else PK && PK.toast(t.message || 'خطا', false, false); })
        .catch(() => PK && PK.toast('خطایی رخ داد؛ دوباره تلاش کنید.', false, false));
    } else if (e.target.closest('.ci-del')) {
      post('/Basket/RemoveItem', { code: id }).then(t => {
        row.classList.add('gone');
        setTimeout(() => { row.remove(); apply(t); }, 450);
        $('cmsg').textContent = '';
      }).catch(() => PK && PK.toast('خطایی رخ داد؛ دوباره تلاش کنید.', false, false));
    }
  });

  // suggestions: add and show the item in the cart
  const sgrid = $('sgrid');
  if (sgrid) sgrid.addEventListener('click', e => {
    const b = e.target.closest('[data-cart-add]'); if (!b) return;
    e.preventDefault();
    post('/Basket/AddToCart', { code: b.dataset.cartAdd, qty: 1 }).then(res => { if (res === 'true') location.reload(); else PK && PK.toast('این محصول در حال حاضر موجود نیست.', false, false); });
  });

  // coupon
  const messages = {
    invald: 'کد تخفیف نامعتبر است.',
    used: 'این کد تخفیف قبلاً استفاده شده است.',
    expired: 'این کد تخفیف منقضی شده است.',
    maxtotal: 'مبلغ سفارش برای استفاده از این کد کافی نیست.',
    notfirstorder: 'این کد مخصوص خرید اول است.',
    login: 'برای استفاده از این کد ابتدا وارد حساب کاربری خود شوید.'
  };
  $('coupon').addEventListener('submit', e => {
    e.preventDefault();
    const v = $('ccode').value.trim(), m = $('cmsg');
    m.classList.remove('ok');
    if (!v) { m.textContent = 'کد تخفیف را وارد کنید.'; return; }
    fetch('/Basket/DiscountRequestPost?coupon=' + encodeURIComponent(v), { credentials: 'same-origin' }).then(r => r.json()).then(res => {
      const key = String(res).toLowerCase();
      if (key === 'true') {
        return fetch('/Basket/CartSummary', { credentials: 'same-origin' }).then(r => r.json()).then(t => {
          apply(t); m.classList.add('ok'); m.textContent = 'کد تخفیف اعمال شد.';
        });
      }
      m.textContent = messages[key] || 'کد تخفیف معتبر نیست.';
    }).catch(() => { m.textContent = 'خطایی رخ داد؛ دوباره تلاش کنید.'; });
  });
})();
