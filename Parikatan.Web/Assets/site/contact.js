// contact: copy the SMS number, and the message form (validation + POST + thank-you state)
(() => {
  const $ = id => document.getElementById(id);
  const fa = s => String(s).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
  const en = s => String(s).replace(/[۰-۹]/g, d => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d)).replace(/[٠-٩]/g, d => '٠١٢٣٤٥٦٧٨٩'.indexOf(d));
  const say = m => window.PK && PK.toast(m, false);

  const copy = $('copy-sms');
  if (copy) copy.addEventListener('click', () => {
    if (navigator.clipboard) navigator.clipboard.writeText('10004969').then(() => say('شماره‌ی سامانه کپی شد: ۱۰۰۰۴۹۶۹'), () => say('شماره‌ی سامانه: ۱۰۰۰۴۹۶۹'));
    else say('شماره‌ی سامانه: ۱۰۰۰۴۹۶۹');
  });

  const form = $('ct-form'); if (!form) return;
  const name = $('c-name'), mobile = $('c-mobile'), topic = $('c-topic'), msg = $('c-msg');
  const digits = () => en(mobile.value).replace(/\D/g, '');
  const rules = {
    'c-name': () => name.value.trim().length < 3 ? 'نام و نام خانوادگی را وارد کنید.' : '',
    'c-mobile': () => { const d = digits(); return !d ? 'شماره موبایل را وارد کنید.' : /^09\d{9}$/.test(d) ? '' : 'شماره موبایل باید ۱۱ رقم و با ۰۹ شروع شود.'; },
    'c-topic': () => topic.value ? '' : 'موضوع پیام را انتخاب کنید.',
    'c-msg': () => msg.value.trim().length < 10 ? 'پیام باید دست‌کم ۱۰ نویسه باشد.' : ''
  };
  const check = (el, quiet) => {
    const fld = el.closest('.fld'), m = rules[el.id]();
    if (quiet && !fld.classList.contains('bad')) return !m;
    fld.querySelector('.f-err').textContent = m; fld.classList.toggle('bad', !!m); fld.classList.toggle('ok', !m);
    return !m;
  };
  mobile.addEventListener('input', () => { const d = digits().slice(0, 11); mobile.value = fa([d.slice(0, 4), d.slice(4, 7), d.slice(7)].filter(Boolean).join(' ')); });
  Object.keys(rules).forEach(id => { const el = $(id); el.addEventListener(el.tagName === 'SELECT' ? 'change' : 'input', () => check(el, true)); });

  const go = form.querySelector('.ct-go');
  form.addEventListener('submit', e => {
    e.preventDefault();
    const bad = Object.keys(rules).map(id => $(id)).filter(el => !check(el));
    if (bad.length) { bad[0].focus(); return; }
    go.classList.add('loading'); go.disabled = true;
    // the stored message is "[topic] text" (the table keeps name, phone and one message field)
    const body = ('[' + topic.value + '] ' + msg.value.trim()).slice(0, 300);
    fetch('/ContactUsForms/SubmitComment', {
      method: 'POST', credentials: 'same-origin',
      headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8', 'X-Requested-With': 'XMLHttpRequest' },
      body: new URLSearchParams({ name: name.value.trim(), phone: digits(), email: '', body }).toString()
    }).then(r => r.json()).then(res => {
      go.classList.remove('loading'); go.disabled = false;
      if (String(res) === 'true') { $('ct-form-view').hidden = true; $('ct-done').hidden = false; }
      else say('ارسال پیام انجام نشد؛ لطفاً دوباره تلاش کنید.');
    }).catch(() => { go.classList.remove('loading'); go.disabled = false; say('ارتباط با سرور برقرار نشد.'); });
  });
  $('ct-again').addEventListener('click', () => {
    form.reset(); document.querySelectorAll('#ct-form .fld').forEach(f => { f.classList.remove('bad', 'ok'); f.querySelector('.f-err').textContent = ''; });
    $('ct-done').hidden = true; $('ct-form-view').hidden = false; name.focus();
  });
})();