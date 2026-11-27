(() => {
    document.querySelectorAll('.js-ans-copy').forEach((btn) => {
        btn.addEventListener('click', async () => {
            const targetId = btn.dataset.target;
            const input = targetId ? document.getElementById(targetId) : null;
            if (!input)
                return;

            try {
                await navigator.clipboard.writeText(input.value);
                btn.classList.replace('btn-outline-secondary', 'btn-success');
                setTimeout(() => {
                    btn.classList.replace('btn-success', 'btn-outline-secondary');
                }, 1200);
            } catch {
                input.select();
            }
        });
    });
})();
