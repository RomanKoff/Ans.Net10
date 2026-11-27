(() => {
    const loginBox = document.getElementById('ansSysLoginBox');
    const loginForm = document.getElementById('ansSysLoginForm');
    const logoutBtn = document.getElementById('ansSysLogoutBtn');
    const accessBar = document.getElementById('ansSysAccessBar');

    function normalizeBase(raw) {
        const clean = (raw || '/api/ans/access').replace(/\/+$/, '');
        return clean.startsWith('/') || clean.startsWith('http') ? clean : `/${clean}`;
    }

    if (loginBox && loginForm) {
        const i18n = JSON.parse(loginBox.dataset.i18n || '{}');
        const xsrfToken = loginBox.dataset.xsrf || '';
        const apiBase = normalizeBase(loginBox.dataset.apiBase);
        const userInput = document.getElementById('ansSysUserInput');
        const keyInput = document.getElementById('ansSysKeyInput');
        const errBox = document.getElementById('ansSysLoginError');
        const submitBtn = document.getElementById('ansSysLoginBtn');

        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            if (!userInput || !keyInput || !errBox || !submitBtn)
                return;

            errBox.classList.add('d-none');
            const origHtml = submitBtn.innerHTML;
            submitBtn.disabled = true;
            submitBtn.innerHTML = `<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>${origHtml}`;

            try {
                const res = await fetch(`${apiBase}/login`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': xsrfToken
                    },
                    body: JSON.stringify({
                        username: userInput.value,
                        key: keyInput.value
                    })
                });

                if (res.ok) {
                    window.location.reload();
                    return;
                }

                if (res.status === 429)
                    errBox.textContent = i18n.errRateLimit || 'Too many attempts.';
                else if (res.status === 401)
                    errBox.textContent = i18n.errInvalid || 'Invalid credentials.';
                else
                    errBox.textContent = i18n.errGeneric || 'Access denied.';

                errBox.classList.remove('d-none');
                keyInput.value = '';
                keyInput.focus();
            } catch {
                errBox.textContent = i18n.errGeneric || 'Network error.';
                errBox.classList.remove('d-none');
            } finally {
                submitBtn.disabled = false;
                submitBtn.innerHTML = origHtml;
            }
        });
    }

    if (logoutBtn && accessBar) {
        const xsrfToken = accessBar.dataset.xsrf || '';
        const apiBase = normalizeBase(accessBar.dataset.apiBase);

        logoutBtn.addEventListener('click', async () => {
            logoutBtn.disabled = true;
            try {
                await fetch(`${apiBase}/logout`, {
                    method: 'POST',
                    headers: { 'RequestVerificationToken': xsrfToken }
                });
                window.location.reload();
            } catch {
                logoutBtn.disabled = false;
            }
        });
    }
})();
