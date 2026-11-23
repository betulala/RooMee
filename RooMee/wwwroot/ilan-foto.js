// @ts-nocheck
// İlan formunda seçilen fotoğrafları biriktirir ve önizleme gösterir.
(function () {
    const MAX = 5;
    const MAX_BOYUT = 3 * 1024 * 1024;
    const IZINLI = ['image/jpeg', 'image/png', 'image/webp'];

    const secici = document.getElementById('YeniFotolar');
    const onizleme = document.getElementById('fotoOnizleme');
    const uyari = document.getElementById('fotoUyari');
    if (!secici || !onizleme || !uyari) return;

    const mevcutSayi = Number(secici.dataset.mevcut || 0);
    let dosyalar = [];

    function guncelle() {
        // Biriken dosyaları seçim kutusunun kendisine geri yaz (form bunu gönderir)
        const dt = new DataTransfer();
        dosyalar.forEach(f => dt.items.add(f));
        secici.files = dt.files;

        // Önizlemeleri çiz
        onizleme.innerHTML = '';
        dosyalar.forEach((f, i) => {
            const kutu = document.createElement('div');
            kutu.className = 'position-relative';

            const img = document.createElement('img');
            img.src = URL.createObjectURL(f);
            img.alt = f.name;
            img.className = 'rounded border';
            img.style.cssText = 'width:110px;height:80px;object-fit:cover';

            const btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'btn btn-sm btn-danger position-absolute top-0 end-0 py-0 px-1';
            btn.textContent = '×';
            btn.title = 'Kaldır';
            btn.onclick = () => { dosyalar.splice(i, 1); uyari.textContent = ''; guncelle(); };

            kutu.append(img, btn);
            onizleme.append(kutu);
        });
    }

    secici.addEventListener('change', () => {
        uyari.textContent = '';
        // Tarayıcı yeni seçimi koyunca eskileri unutur; biz eskileri listede tutuyoruz
        for (const f of Array.from(secici.files)) {
            if (!IZINLI.includes(f.type)) {
                uyari.textContent = `${f.name}: sadece JPG, PNG veya WEBP yüklenebilir.`;
                continue;
            }
            if (f.size > MAX_BOYUT) {
                uyari.textContent = `${f.name}: en fazla 3 MB olabilir.`;
                continue;
            }
            if (mevcutSayi + dosyalar.length >= MAX) {
                uyari.textContent = `Bir ilanda en fazla ${MAX} fotoğraf olabilir.`;
                break;
            }
            dosyalar.push(f);
        }
        guncelle();
    });
})();