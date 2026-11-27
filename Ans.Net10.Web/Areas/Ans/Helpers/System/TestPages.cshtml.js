(() => {
    async function checkUrl(url) {
        try {
            const res1 = await fetch(url, { method: 'GET', cache: 'no-store' });
            return res1.status;
        } catch {
            return 'ERR';
        }
    }

    document.querySelectorAll('.js-page-check').forEach((e1) => {
        const info1 = e1.querySelector('span');
        const url1 = e1.dataset.path;
        if (!info1 || !url1)
            return;

        checkUrl(url1).then(status1 => {
            switch (status1) {
                case 200:
                    info1.innerHTML = '&nbsp;—&nbsp;<em class="text-success">OK</em>';
                    break;
                case 404:
                    info1.innerHTML = '&nbsp;—&nbsp;<em class="text-danger">404</em>';
                    break;
                case 500:
                    info1.innerHTML = '&nbsp;—&nbsp;<em class="text-danger">500</em>';
                    break;
                default:
                    info1.innerHTML = `&nbsp;—&nbsp;<em class="text-danger">${status1}</em>`;
                    break;
            }
        });
    });
})();
