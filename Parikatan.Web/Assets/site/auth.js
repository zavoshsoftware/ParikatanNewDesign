// login (mobile number) + verification (5-digit code); both are plain server form posts, this only
// formats the input, assembles the one-time code and drives the resend timer.
(() => {
  const fa = s => String(s).replace(/\d/g, d => '۰۱۲۳۴۵۶۷۸۹'[d]);
  const en = s => String(s).replace(/[۰-۹]/g, d => '۰۱۲۳۴۵۶۷۸۹'.indexOf(d)).replace(/[٠-٩]/g, d => '٠١٢٣٤٥٦٧٨٩'.indexOf(d));

  // ---------- login: mobile number ----------
  const form = document.getElementById('login-form');
  if (form) {
    const input = document.getElementById('mobile'), hidden = document.getElementById('UserCellNumber');
    const err = document.getElementById('phone-err'), field = input.parentElement;
    const digits = v => { let d = en(v).replace(/\D/g, ''); if (d.startsWith('0098')) d = '0' + d.slice(4); else if (d.startsWith('98') && d.length > 10) d = '0' + d.slice(2); else if (d.startsWith('9')) d = '0' + d; return d.slice(0, 11); };
    const pretty = d => fa([d.slice(0, 4), d.slice(4, 7), d.slice(7, 11)].filter(Boolean).join(' '));
    input.addEventListener('input', () => {
      input.value = pretty(digits(input.value));
      if (err.textContent && /^09\d{9}$/.test(digits(input.value))) { err.textContent = ''; field.classList.remove('bad'); }
    });
    form.addEventListener('submit', e => {
      const d = digits(input.value);
      if (!d) { e.preventDefault(); err.textContent = 'لطفاً شماره موبایل خود را وارد کنید.'; }
      else if (!/^09\d{9}$/.test(d)) { e.preventDefault(); err.textContent = 'شماره موبایل باید ۱۱ رقم و با ۰۹ شروع شود.'; }
      else {
        hidden.value = d;
        err.textContent = ''; field.classList.remove('bad');
        form.querySelector('.auth-btn').classList.add('loading');
        return;   // normal POST
      }
      field.classList.add('bad'); input.focus();
    });
    setTimeout(() => input.focus({ preventScroll: true }), 300);
  }

  // ---------- verify: 5-digit code ----------
  const otpForm = document.getElementById('otp-form');
  if (otpForm) {
    const boxes = [...document.querySelectorAll('#otp input')], wrap = document.getElementById('otp');
    const err = document.getElementById('otp-err'), btn = otpForm.querySelector('.auth-btn');
    const hidden = document.getElementById('ActivationCode');
    const code = () => boxes.map(b => b.value).join('');
    const sync = () => { boxes.forEach(b => b.classList.toggle('fill', !!b.value)); btn.disabled = code().length !== 5; };
    const fill = (from, str) => { str.split('').forEach((ch, k) => { if (boxes[from + k]) boxes[from + k].value = fa(ch); }); };
    let submitted = false;
    const submit = () => {
      if (submitted || code().length !== 5) return;
      submitted = true; hidden.value = en(code()); btn.classList.add('loading'); otpForm.submit();
    };
    boxes.forEach((b, i) => {
      b.addEventListener('input', () => {
        const v = en(b.value).replace(/\D/g, '');
        if (v.length > 1) { fill(i, v.slice(0, 5 - i)); }         // pasted or auto-filled (one-time-code)
        else b.value = v ? fa(v) : '';
        wrap.classList.remove('bad'); err.textContent = '';
        const next = boxes.find(x => !x.value); (v ? (next || boxes[4]) : b).focus();
        sync(); if (code().length === 5) submit();
      });
      b.addEventListener('keydown', e => {
        if (e.key === 'Backspace' && !b.value && i) { boxes[i - 1].value = ''; boxes[i - 1].focus(); sync(); e.preventDefault(); }
        if (e.key === 'ArrowLeft' && i < 4) boxes[i + 1].focus();
        if (e.key === 'ArrowRight' && i) boxes[i - 1].focus();
      });
      b.addEventListener('paste', e => {
        const v = en((e.clipboardData || window.clipboardData).getData('text')).replace(/\D/g, ''); if (!v) return;
        e.preventDefault(); fill(i, v.slice(0, 5 - i)); wrap.classList.remove('bad'); err.textContent = '';
        sync(); (boxes.find(x => !x.value) || boxes[4]).focus(); if (code().length === 5) submit();
      });
      b.addEventListener('focus', () => b.select());
    });
    otpForm.addEventListener('submit', e => { if (!submitted) { e.preventDefault(); submit(); } });

    // resend timer; resending posts the number again, the server sends a new code and returns to this page
    const left = document.getElementById('otp-left'), timer = document.getElementById('otp-timer'), resend = document.getElementById('otp-resend');
    let t = 120;
    const tick = () => {
      left.textContent = fa(String(Math.floor(t / 60)).padStart(2, '0') + ':' + String(t % 60).padStart(2, '0'));
      if (t-- <= 0) { clearInterval(iv); timer.hidden = true; resend.hidden = false; }
    };
    tick(); const iv = setInterval(tick, 1000);
    resend.addEventListener('click', () => document.getElementById('resend-form').submit());

    sync(); setTimeout(() => boxes[0].focus({ preventScroll: true }), 300);
  }
})();
