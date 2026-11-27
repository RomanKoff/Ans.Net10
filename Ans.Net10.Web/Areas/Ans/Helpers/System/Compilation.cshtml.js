(() => {
    const root = document.getElementById('ansCompileRoot');
    const reloadBtn = document.getElementById('ansCompileReloadBtn');
    const batchBtn = document.getElementById('ansCompileBatchBtn');
    const appCodeBox = document.getElementById('ansCompileAppCodeBox');
    const batchBox = document.getElementById('ansCompileBatchBox');
    if (!root || !reloadBtn || !batchBtn || !appCodeBox || !batchBox)
        return;

    const i18n = JSON.parse(root.dataset.i18n || '{}');
    const xsrfToken = root.dataset.xsrf || '';
    const rawBase = (root.dataset.apiBase || '/api/ans/compile').replace(/\/+$/, '');
    const apiBase = rawBase.startsWith('/') || rawBase.startsWith('http')
        ? rawBase
        : `/${rawBase}`;

    function fmt(tpl, ...args) {
        return String(tpl ?? '').replace(/\{(\d+)\}/g, (_, i) => args[i] ?? '');
    }

    function esc(s) {
        return String(s ?? '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    function renderErrorsTable(errors) {
        if (!errors || !errors.length)
            return '';
        const rows = errors.map(e => `<tr>
            <td class="font-monospace text-danger">${esc(e.errorCode)}</td>
            <td class="font-monospace small">${esc(e.filePath)}</td>
            <td class="font-monospace text-nowrap">${e.line}:${e.column}</td>
            <td>${esc(e.message)}</td>
        </tr>`).join('');
        return `<div class="table-responsive mt-3"><table class="table table-sm table-bordered mb-0">
            <thead class="table-light"><tr><th>${esc(i18n.code)}</th><th>${esc(i18n.file)}</th><th>${esc(i18n.lineCol)}</th><th>${esc(i18n.desc)}</th></tr></thead>
            <tbody>${rows}</tbody></table>
        </div>`;
    }

    async function postJson(action, btn) {
        const origHtml = btn.innerHTML;
        btn.disabled = true;
        btn.innerHTML = `<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>${origHtml}`;
        try {
            const res = await fetch(`${apiBase}/${action}`, {
                method: 'POST',
                headers: { 'RequestVerificationToken': xsrfToken }
            });
            return res.ok
                ? await res.json()
                : null;
        } finally {
            btn.disabled = false;
            btn.innerHTML = origHtml;
        }
    }

    reloadBtn.addEventListener('click', async () => {
        const data = await postJson('reload', reloadBtn);
        if (!data)
            return;
        const ok = data.success;
        const dt = new Date(data.compiledAtUtc).toISOString().replace('T', ' ').substring(0, 19);
        appCodeBox.innerHTML = `<div class="card mb-4 ${ok ? 'border-success' : 'border-danger'}">
            <div class="card-header d-flex justify-content-between align-items-center ${ok ? 'bg-success-subtle text-success-emphasis' : 'bg-danger-subtle text-danger-emphasis'}">
                <strong>${esc(ok ? i18n.asmActive : i18n.asmError)}</strong>
                <span class="small">${esc(fmt(i18n.builtIn, data.elapsedMilliseconds, dt))}</span>
            </div>
            <div class="card-body">
                <div class="d-flex flex-wrap gap-2">
                    <span class="badge text-bg-secondary">${esc(fmt(i18n.filesCs, data.sourceFilesCount))}</span>
                    <span class="badge text-bg-info">${esc(fmt(i18n.exportedTypes, data.exportedTypesCount))}</span>
                </div>
                ${renderErrorsTable(data.errors)}
            </div>
        </div>`;
    });

    batchBtn.addEventListener('click', async () => {
        const data = await postJson('batch', batchBtn);
        if (!data)
            return;
        const ok = data.failedCount === 0;
        batchBox.innerHTML = `<div class="card mb-4 ${ok ? 'border-success' : 'border-danger'}">
            <div class="card-header d-flex justify-content-between align-items-center ${ok ? 'bg-success-subtle text-success-emphasis' : 'bg-danger-subtle text-danger-emphasis'}">
                <strong>${esc(ok ? i18n.batchSuccess : fmt(i18n.batchError, data.failedCount, data.totalViewsCount))}</strong>
                <span class="small">${esc(fmt(i18n.buildTime, data.elapsedMilliseconds))}</span>
            </div>
            <div class="card-body">
                <div class="d-flex flex-wrap gap-2">
                    <span class="badge text-bg-secondary">${esc(fmt(i18n.total, data.totalViewsCount))}</span>
                    <span class="badge text-bg-success">${esc(fmt(i18n.succeeded, data.succeededCount))}</span>
                    <span class="badge text-bg-danger">${esc(fmt(i18n.failed, data.failedCount))}</span>
                </div>
                ${renderErrorsTable(data.errors)}
            </div>
        </div>`;
    });

})();
