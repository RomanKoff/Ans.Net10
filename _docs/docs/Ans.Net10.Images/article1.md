# Как правильно обрабатывать тяжелый CPU-bound (pHash, ресайз) в веб-приложениях?

Если вы вызываете ленивое свойство PerceptualHash или метод тяжелого ресайза внутри ASP.NET Core контроллера (или Minimal API), этот запрос на несколько сотен миллисекунд монополизирует поток процессора.

Чтобы тяжелая математика ImageMagick не приводила к микрофризам веб-сервера, в вызывающем коде (на уровне сервисов приложения, а не внутри вашей библиотеки картинок) вычисления уводят в фоновый поток через Task.Run:
csharp

```csharp
// Пример вызова вашей библиотеки в контроллере/сервисе:
public async Task<IActionResult> UploadPhoto(IFormStream stream)
{
    // 1. Асинхронно сохраняем/читаем файл (освобождаем I/O)
    var mediaInfo = new MediaFileInfo("uploaded.jpg"); 

    // 2. Уводим тяжелую математику ресайза и pHash в фоновый поток Task.Run,
    // чтобы не блокировать основной поток обработки HTTP-запросов
    var result = await Task.Run(() => 
    {
        using var helper = new ImageHelper(mediaInfo);
        
        // Здесь лениво посчитается pHash и сделается ресайз
        var hash = mediaInfo.PerceptualHash; 
        var thumb = helper.Image.GetResizeInside(300, 300, true, false);
        
        return new { Hash = hash, Image = thumb };
    });

    // 3. Асинхронно пишем результат
    await result.Image.WriteAsync("thumb.jpg");
    return Ok(result.Hash);
}
```