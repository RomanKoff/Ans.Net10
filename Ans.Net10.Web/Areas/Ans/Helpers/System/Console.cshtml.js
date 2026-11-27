(() => {
    const terminal = document.getElementById('ansConsoleTerminal');
    const levelFilter = document.getElementById('ansConsoleLevelFilter');
    const liveCheck = document.getElementById('ansConsoleLiveCheck');
    const clearBtn = document.getElementById('ansConsoleClearBtn');

    if (!terminal || !levelFilter || !liveCheck || !clearBtn)
        return;

    let lastId = parseInt(terminal.dataset.lastId || '0', 10);
    const xsrfToken = terminal.dataset.xsrf || '';
    const rawBase = (terminal.dataset.apiBase || '/api/ans/console').replace(/\/+$/, '');
    const apiBase = rawBase.startsWith('/') || rawBase.startsWith('http') ? rawBase : `/${rawBase}`;
    const lvlNames = [
        'VRB',
        'DBG',
        'INF',
        'WRN',
        'ERR',
        'FTL'
    ];
    const lvlClasses = [
        'text-body-secondary',
        'text-body-secondary',
        'text-success-emphasis',
        'text-warning-emphasis',
        'text-danger-emphasis',
        'badge text-bg-danger'
    ];

    function buildUrl(action, extra = {}) {
        const q = new URLSearchParams(extra);
        const qs = q.toString();
        return qs ? `${apiBase}/${action}?${qs}` : `${apiBase}/${action}`;
    }

    function applyFilter() {
        const min = levelFilter.value ? parseInt(levelFilter.value, 10) : 0;
        for (const row of terminal.children) {
            const lvl = parseInt(row.dataset.lvl || '0', 10);
            row.classList.toggle('d-none', lvl < min);
        }
    }

    function esc(s) {
        return String(s ?? '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    }

    async function pollUpdates() {
        if (!liveCheck.checked)
            return;
        try {
            const res = await fetch(buildUrl('updates', { afterId: lastId }));
            if (!res.ok)
                return;
            const items = await res.json();
            if (!items.length)
                return;
            const min = levelFilter.value ? parseInt(levelFilter.value, 10) : 0;
            for (const item of items) {
                lastId = Math.max(lastId, item.id);
                const code = lvlNames[item.level] || 'UNK';
                const cls = lvlClasses[item.level] || 'text-body-secondary';
                const div = document.createElement('div');
                div.className = 'border-bottom border-secondary-subtle py-1 text-break';
                div.dataset.lvl = item.level;
                if (item.level < min)
                    div.classList.add('d-none');
                const d = new Date(item.timestamp);
                const ts = d.toTimeString().split(' ') + '.' + String(d.getMilliseconds()).padStart(3, '0');
                div.innerHTML = `<span class="text-body-secondary">${ts}</span> `
                    + `<span class="fw-bold ${cls}">[${code}]</span> `
                    + (item.sourceContext ? `<span class="text-info-emphasis">[${esc(item.sourceContext)}]</span> ` : '')
                    + `<span style="white-space: pre-wrap;">${esc(item.message)}</span>`
                    + (item.exception ? `<div class="text-danger-emphasis border-start border-2 border-danger ps-3 mt-1" style="white-space: pre-wrap;">${esc(item.exception)}</div>` : '');
                terminal.prepend(div);
            }
        } catch { }
    }

    levelFilter.addEventListener('change', applyFilter);

    clearBtn.addEventListener('click', async () => {
        await fetch(buildUrl('clear'), {
            method: 'POST',
            headers: { 'RequestVerificationToken': xsrfToken }
        });
        terminal.innerHTML = '';
    });

    applyFilter();
    terminal.scrollTop = 0;
    setInterval(pollUpdates, 2000);

})();
