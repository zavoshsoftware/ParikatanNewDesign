// checkout: shipping form (validation, province -> city from the server) and order submit.
// Submitting calls the existing GET /Basket/Finalize, which creates the order and returns the gateway url.
(() => {
  const fa = n => String(n).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
  const en = s => String(s).replace(/[۰-۹]/g, d => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d)).replace(/[٠-٩]/g, d => '٠١٢٣٤٥٦٧٨٩'.indexOf(d));
  const $ = id => document.getElementById(id);
  const form = $('co-form'); if (!form) return;

  const name = $('f-name'), mobile = $('f-mobile'), postal = $('f-postal'), address = $('f-address');
  const prov = $('f-province'), city = $('f-city');
  const go = $('co-go'), errBox = $('co-err');
  const digits = el => en(el.value).replace(/\D/g, '');

  // province -> cities (server: POST /Basket/FillCities returns [{Text, Value}])
  const loadCities = (provinceId, keep) => {
    city.disabled = true; city.innerHTML = '<option value="">در حال بارگذاری…</option>';
    if (!provinceId) { city.innerHTML = '<option value="">ابتدا استان را انتخاب کنید</option>'; return Promise.resolve(); }
    return fetch('/Basket/FillCities', {
      method: 'POST', credentials: 'same-origin',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8', 'X-Requested-With': 'XMLHttpRequest' },
      body: 'id=' + encodeURIComponent(provinceId)
    }).then(r => r.json()).then(list => {
      city.innerHTML = '<option value="">انتخاب شهر</option>' + list.map(c => `<option value="${c.Value}">${c.Text}</option>`).join('');
      city.disabled = !list.length;
      if (keep) city.value = keep;
    }).catch(() => { city.innerHTML = '<option value="">خطا در دریافت شهرها</option>'; });
  };
  prov.addEventListener('change', () => { loadCities(prov.value).then(() => check(city, true)); });

  // validation
  const rules = {
    'f-name': () => name.value.trim().length < 3 ? 'نام و نام خانوادگی را کامل وارد کنید.' : '',
    'f-mobile': () => { const d = digits(mobile); return !d ? 'شماره موبایل را وارد کنید.' : /^09\d{9}$/.test(d) ? '' : 'شماره موبایل باید ۱۱ رقم و با ۰۹ شروع شود.'; },
    'f-postal': () => { const d = digits(postal); return !d || d.length === 10 ? '' : 'کد پستی باید ۱۰ رقم باشد.'; },
    'f-province': () => prov.value ? '' : 'استان را انتخاب کنید.',
    'f-city': () => city.value ? '' : 'شهر را انتخاب کنید.',
    'f-address': () => address.value.trim().length < 10 ? 'آدرس را کامل وارد کنید (خیابان، کوچه، پلاک).' : ''
  };
  const check = (el, quiet) => {
    const fld = el.closest('.fld'), msg = rules[el.id]();
    if (quiet && !fld.classList.contains('bad')) { fld.classList.toggle('ok', !msg && !!el.value); return !msg; }
    fld.querySelector('.f-err').textContent = msg; fld.classList.toggle('bad', !!msg); fld.classList.toggle('ok', !msg && !!el.value);
    return !msg;
  };
  const fmtMobile = () => { const d = digits(mobile).slice(0, 11); mobile.value = fa([d.slice(0, 4), d.slice(4, 7), d.slice(7)].filter(Boolean).join(' ')); };
  const fmtPostal = () => { const d = digits(postal).slice(0, 10); postal.value = fa(d.length > 5 ? d.slice(0, 5) + '-' + d.slice(5) : d); };
  mobile.addEventListener('input', fmtMobile); postal.addEventListener('input', fmtPostal);
  Object.keys(rules).forEach(id => {
    const el = $(id);
    el.addEventListener(el.tagName === 'SELECT' ? 'change' : 'input', () => check(el, true));
    el.addEventListener('blur', () => { if (el.value) check(el); });
  });

  // prefill: the last address used on this device
  let saved = null; try { saved = JSON.parse(localStorage.getItem('pk-address')); } catch (e) {}
  if (saved) {
    if (!name.value) name.value = saved.name || '';
    postal.value = saved.postal || ''; address.value = saved.address || '';
    if (saved.province) { prov.value = saved.province; loadCities(prov.value, saved.city); }
  }
  fmtMobile(); fmtPostal();

  const showError = html => { errBox.innerHTML = html; errBox.hidden = !html; };
  const busy = on => { go.classList.toggle('loading', on); go.disabled = on; $('co-wait').hidden = !on; };

  form.addEventListener('submit', e => {
    e.preventDefault();
    showError('');
    const bad = Object.keys(rules).map($).filter(el => !check(el));
    if (bad.length) { bad[0].focus(); bad[0].closest('.co-card').scrollIntoView({ behavior: 'smooth', block: 'start' }); return; }

    try { localStorage.setItem('pk-address', JSON.stringify({ name: name.value.trim(), postal: postal.value, province: prov.value, city: city.value, address: address.value.trim() })); } catch (err) {}

    busy(true);
    const q = new URLSearchParams({
      notes: '', cellnumber: digits(mobile), postal: digits(postal), address: address.value.trim(),
      city: city.value, fullname: name.value.trim(), bank: 'zarinpal'
    });
    fetch('/Basket/Finalize?' + q.toString(), { credentials: 'same-origin', headers: { 'X-Requested-With': 'XMLHttpRequest' } })
      .then(r => r.json())
      .then(result => {
        result = String(result);
        if (result.indexOf('nonstock') === 0 || result.indexOf('nonstock') > -1) {
          const names = result.split('|').slice(1).filter(Boolean).map(n => '«' + n.replace(/</g, '&lt;') + '»').join('، ');
          showError('موجودی ' + names + ' کمتر از تعداد انتخابی شماست. لطفاً به <a href="/basket">سبد خرید</a> بروید و تعداد را اصلاح کنید.');
          busy(false);
        } else if (result === 'emptyBasket') {
          location.href = '/basket';
        } else if (result !== 'false' && result) {
          location.href = result;   // payment gateway
        } else {
          showError('خطایی رخ داده است. لطفاً دوباره تلاش کنید.');
          busy(false);
        }
      })
      .catch(() => { showError('ارتباط با سرور برقرار نشد. لطفاً دوباره تلاش کنید.'); busy(false); });
  });
})();
