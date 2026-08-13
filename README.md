# CB Election Ticker Control

Cumhurbaşkanlığı seçimi için Viz Artist 3.14, Viz Engine ve Viz Ticker Service kullanan yerel WinForms kontrol uygulaması.

Bu proje seçim günü geldiğinde sistemin nasıl kurulduğunu, verinin nasıl aktığını ve olası sorunlarda nelerin kontrol edilmesi gerektiğini hatırlamak için hazırlanmıştır.

> Bu sürüm yalnızca yerel test sistemi içindir. Yayın sistemine veya başka bir Viz Engine’e bağlanmak amacıyla kullanılmamalıdır.

## Projenin amacı

Ekranın sol tarafında aynı anda iki aday gösterilir:

- Sayfa 1: Aday 1 ve Aday 2
- Sayfa 2: Aday 3 ve Aday 4

Sayfalar tek bir Viz direktörü üzerinden kayarak değiştirilir:

- `START NORMAL` → Aday 3/4
- `START REVERSE` → Aday 1/2

Seçim verileri JSON dosyasından okunur, Viz Ticker XML biçimine dönüştürülür ve Viz Ticker Service üzerinden sahneye gönderilir.

## Güvenlik sınırları

Uygulama yalnızca bu bilgisayardaki yerel test sistemini kullanır.

| Ayar | Sabit değer |
|---|---|
| Ticker | `CBSECIMLOCALTEST2026` |
| Grup | `local_preview_ankara` |
| Element key | `1` |
| Tasarım | `CityResult` |
| Viz Engine | `127.0.0.1:6100` |
| Şehir | Ankara, `IlId = 6` |
| JSON | `canli_test.json` |

`LocalVizEngineClient` başka bir IP adresine bağlanamaz. Viz Engine tarafında yalnızca aşağıdaki iki komuta izin verilir:

```text
MAIN_SCENE*STAGE*DIRECTOR*SOL_4_ADAY_SAYFA START NORMAL
MAIN_SCENE*STAGE*DIRECTOR*SOL_4_ADAY_SAYFA START REVERSE
```

## Kullanılan teknolojiler

- C# WinForms
- .NET Framework 4.7.2
- Viz Artist 3.14
- Viz Engine
- Viz Ticker Service
- `VIZTICKERLib` COM kütüphanesi
- `FileSystemWatcher`
- Yerel TCP bağlantısı

## Veri akışı

```text
canli_test.json
        ↓
ElectionJsonReader
        ↓
CityTickerData
        ↓
TickerXmlBuilder
        ↓
VizTickerClient
        ↓
Viz Ticker Service
        ↓
CityScroller
        ↓
CityResult
```

Animasyon kontrolü veri yolundan ayrıdır:

```text
5 saniyelik WinForms Timer
        ↓
LocalVizEngineClient
        ↓
127.0.0.1:6100
        ↓
SOL_4_ADAY_SAYFA
        ↓
NORMAL / REVERSE
```

Bu ayrım sayesinde JSON güncellemesi ile sayfa animasyonu birbirini doğrudan yönetmez.

## Proje dosyaları

### `Form1.cs`

Ana kullanıcı arayüzünü yönetir.

Görevleri:

- Yerel Ticker Service bağlantısını başlatmak
- JSON dosya izleyicisini açmak
- XML önizlemek
- Manuel veri göndermek
- Otomatik JSON güncellemelerini işlemek
- 1/2 ve 3/4 sayfalarını manuel göstermek
- 5 saniyelik otomatik sayfa geçişini yönetmek
- Ticker durumunu okumak
- Gerektiğinde yerel elementi kontrollü şekilde yeniden oluşturmak
- Form kapanırken timer ve bağlantıları temizlemek

### `VizTickerClient.cs`

Viz Ticker Service COM işlemlerini yönetir.

Görevleri:

- Yalnızca `CBSECIMLOCALTEST2026` ticker’ına bağlanmak
- Viz iletişimini başlatmak
- XML güvenlik doğrulaması yapmak
- İlk çalışmada grup ve element oluşturmak
- Normal güncellemelerde `UpdateElement` kullanmak
- Aynı XML tekrar geldiyse gereksiz güncellemeyi atlamak
- Her işlemden sonra elementi geri okuyarak doğrulamak
- COM çağrılarını tek kilit üzerinden seri çalıştırmak

Normal canlı kullanımda grup silinmez.

### `TickerXmlBuilder.cs`

Okunan seçim verisini Viz Ticker XML biçimine dönüştürür.

Şu alanları üretir:

```text
CityName

c1_name c1_pct c1_vote
c2_name c2_pct c2_vote
c3_name c3_pct c3_vote
c4_name c4_pct c4_vote
```

Yüzdeler Türkçe biçimde hazırlanır:

```text
% 48,1
```

Oy sayıları binlik ayraçla hazırlanır:

```text
12.345.678 OY
```

XML’e bilerek `<ttl>` eklenmez. Ticker Service geri okumada bunu `-1` olarak gösterebilir. `-1`, elementin süresiz çalışması anlamına gelir.

### `ElectionJsonReader.cs`

JSON dosyasını okuyup Ankara verisini bulur.

Aday sırası sabittir:

1. `AdayId = 51` — Erdoğan
2. `AdayId = 75` — Kılıçdaroğlu
3. `AdayId = 73` — İnce
4. `AdayId = 76` — Oğan

Eksik şehir veya eksik aday varsa hata oluşturur ve hatalı veri gönderilmez.

Ayrıca şu tür bozuk Türkçe metinleri düzeltmeye çalışır:

```text
ERDOÄžAN
KILIÃ‡DAROÄžLU
```

### `LocalVizEngineClient.cs`

Yalnızca yerel Viz Engine’e direktör komutu gönderir.

Özellikleri:

- Adres sabittir: `127.0.0.1:6100`
- Yalnızca iki izinli komut gönderilebilir
- Komutlar `SemaphoreSlim` ile seri çalıştırılır
- Bağlantı koparsa bir kez yeniden bağlanmayı dener
- Form kapanırken bağlantı kapatılır

## JSON dosyasının otomatik izlenmesi

Ticker bağlantısı başarıyla kurulduktan sonra `canli_test.json` izlenmeye başlanır.

Dosya kaydedildiğinde:

1. `FileSystemWatcher` değişikliği algılar.
2. 400 ms debounce uygulanır.
3. JSON dosyası okunur.
4. Okuma başarısız olursa artan beklemeyle beş kez denenir.
5. JSON sağlamsa XML oluşturulur.
6. Ticker Service’teki element `UpdateElement` ile güncellenir.
7. Güncellenen element tekrar okunarak doğrulanır.
8. Gönderilen ve geri okunan XML ekranda gösterilir.

Yarım yazılmış veya geçersiz JSON sahneye gönderilmez. Böyle bir durumda son doğru yayın verisi korunur.

Uygulama kapalıysa `FileSystemWatcher` çalışmaz. Bu nedenle 7/24 kullanım sırasında kontrol uygulamasının açık kalması gerekir.

## Aynı verinin tekrar gönderilmemesi

`VizTickerClient`, en son başarılı element XML’ini saklar.

Yeni XML önceki XML ile aynıysa ve element Ticker Service’te hâlâ bulunuyorsa:

```text
DEĞİŞİKLİK YOK
```

sonucu döner ve gereksiz `UpdateElement` çağrısı yapılmaz.

## Viz Artist sahne yapısı

Yaklaşık sahne ağacı:

```text
ticker_templates
  CityResult
    Background
    Header
      CityName
    Candidates
      Page12
        Candidate1
        Candidate2
      Page34
        Candidate3
        Candidate4

CBOutput
  CityScroller
    cache
    SrcGrp
    SrcGrp
```

İki `SrcGrp` görülmesi tek başına hata değildir. Scroller çalışma ve önbellek yapısı nedeniyle birden fazla kaynak grup oluşturabilir.

## CityScroller ayarları

```text
Element source: CBSECIMLOCALTEST2026
Layer: CBLayer
Active: on
```

`CBLayer` direktöründe şu stop point’ler bulunmalıdır:

```text
O
CBSECIMLOCALTEST2026
```

Önemli kurallar:

- `ticker_templates` sahne ağacının üst kısmında bulunmalıdır.
- `CityResult` kökünde `ControlObject` olmalıdır.
- Scroller container adı, element source ile aynı olmamalıdır.
- `CityScroller` altında elle container oluşturulmamalıdır.
- Scroller kendi `cache` ve `SrcGrp` yapısını üretir.

## Control alanları

Ticker tarafından güncellenen bütün metin alanları düz `text` olmalıdır.

```text
CityName
c1_name c1_pct c1_vote
c2_name c2_pct c2_vote
c3_name c3_pct c3_vote
c4_name c4_pct c4_vote
```

ControlText alanlarında `Use formatted text` açık olup tip `richtext` olursa şu hatalar görülebilir:

```text
property setting failed
Part not found
```

Şehir alanının ControlObject bilgisi:

```text
Field Identifier: city
Description: CityName
```

Bu eski Ticker sürümünde çalışan XML etiketi `CityName` olduğu için builder bu etiketi kullanır.

## Görsel alanları

JSON okuyucu aşağıdaki görsel alanlarını okuyabilir:

```text
c1_img
c2_img
c3_img
c4_img
```

İzin verilen yol başlangıcı:

```text
IMAGE*/HT_SECIM/C_EKRAN/
```

Ancak mevcut `TickerXmlBuilder`, görsel alanlarını henüz XML’e eklememektedir. Aday fotoğrafları şu anda sahnedeki varsayılan değerlerden gelir.

Canlı kullanımda fotoğrafların veriyle değişmesi gerekiyorsa görsel alanları daha sonra güvenli doğrulamayla builder’a eklenmelidir.

## Sayfa animasyonu

Tek direktör kullanılır:

```text
SOL_4_ADAY_SAYFA
```

Yönler:

```text
NORMAL  → Aday 3/4
REVERSE → Aday 1/2
```

Form üzerindeki manuel düğmeler:

```text
1/2 GÖSTER
3/4 GÖSTER
```

Otomatik geçiş checkbox ile açılıp kapatılır:

```text
5 SN OTOMATİK SAYFA DEĞİŞİMİ
```

Checkbox açıldığında ilk komut beş saniye sonra Aday 3/4 sayfasını gösterir. Sonraki komutlarda yön değiştirilir.

Direktör komutu hata verirse otomatik sayfa geçişi durdurulur.

## Birden fazla bant animasyonu

Gerçek yayın sahnesinde üst bant, alt bant ve aday sayfaları ayrı direktörlerde tutulmalıdır.

Örnek:

```text
SOL_4_ADAY_SAYFA
UST_BANT
ALT_BANT
```

Her direktör farklı container ve özellikleri hareket ettirmelidir. Aynı nesnenin aynı özelliğini iki farklı direktörün aynı anda değiştirmesi önlenmelidir.

5–10 saniyelik timer aralıkları Viz Engine için düşük komut yoğunluğudur. Animasyon süresi timer aralığından kısa olduğu sürece birden fazla bağımsız direktör birlikte çalışabilir.

## Güncellemeden sonra eski verinin bir kez görünmesi

JSON güncellendikten sonra XML kutusunda yeni veri hemen görülebilir fakat ekranda eski değer bir kez daha gösterilebilir.

Muhtemel akış:

```text
Mevcut SrcGrp → eski veri
Sıradaki SrcGrp → yeni veri
```

Ticker Service güncellenmiş olsa da Scroller daha önce oluşturduğu mevcut kopyayı bir kez daha gösterebilir. Yeni oluşturulan kopyada güncel değer görünür.

Bu durum veri kaybı değildir. Scroller önbellek ve kuyruk davranışından kaynaklanan bir görsel gecikmedir.

## Ticker Service notu

Bu bilgisayardaki test sırasında Ticker Service açık görünmesine rağmen Scroller’a veri ulaşmadığı bir durum yaşandı.

Çözüm:

1. Ticker Service kapatıldı.
2. Yönetici olarak yeniden başlatıldı.
3. Kontrol uygulaması yeniden bağlandı.
4. Grup ve element tekrar okundu.
5. `CityResult` sahneye geldi.

Ticker durum çıktısında sağlıklı örnek:

```text
GRUP LİSTESİ:
local_preview_ankara

AKTİF GRUP:
local_preview_ankara

TTL:
-1
```

Ticker Service çalışıyor görünmesine rağmen `SrcGrp` altında `CityResult` oluşmuyorsa önce servis iletişimi kontrol edilmelidir.

## Tanı ve kurtarma düğmeleri

### Ticker durumunu oku

Bu düğme yalnızca mevcut durumu okur:

- Grup listesi
- Aktif grup
- Yerel grup XML’i

Herhangi bir element silmez veya değiştirmez.

### Yerel elementi bir kez yenile

Bu düğme yalnızca:

```text
local_preview_ankara / key=1
```

elementini silip `PutElement` ile tekrar ekler.

Normal canlı güncelleme yöntemi değildir. Yalnızca kontrollü kurtarma amacıyla kullanılmalıdır.

Canlıda sürekli `DeleteElement + PutElement` yapılması görüntü boşluğu veya Scroller kuyruğu problemi oluşturabilir.

## Kullanılmaması gereken işlemler

Normal veri güncellemesinde şunlar kullanılmamalıdır:

```text
DeleteGroup
Clear
ClearAll
Reinitialize
Sahne reload
Her güncellemede DeleteElement + PutElement
```

Normal canlı veri güncellemesi:

```text
UpdateElement
```

olmalıdır.

## Seçim günü başlangıç kontrol listesi

1. Viz Engine’in yalnızca yerel makinede çalıştığını doğrula.
2. Doğru sahneyi aç.
3. `CityScroller` kaynağını kontrol et.
4. `CBLayer` direktörü ve stop point’leri kontrol et.
5. Ticker Service’i yönetici olarak çalıştır.
6. `CBElectionTickerControl` uygulamasını aç.
7. `YEREL TEST SERVİSİNE BAĞLAN` düğmesine bas.
8. Bağlantı durumunun yeşil olduğunu doğrula.
9. `TICKER DURUMUNU OKU` ile aktif grubu kontrol et.
10. JSON’da küçük bir test değişikliği yap.
11. Gönderilen ve geri okunan XML’i karşılaştır.
12. Değerin sahneye geldiğini doğrula.
13. Manuel 1/2 ve 3/4 düğmelerini test et.
14. Otomatik sayfa checkbox’ını aç.
15. Viz Engine ve Ticker Service loglarını izle.

## Bilinen eksikler

Mevcut sürümde henüz bulunmayan özellikler:

- Ticker Service yeniden başlarsa otomatik COM reconnect
- Artan beklemeli sürekli bağlantı kurtarma
- Dönen dosya logları
- Uygulamanın Windows ile otomatik başlaması
- Sağlık kontrolü ve watchdog
- Görsel alanlarının XML’e eklenmesi
- Şehir seçimi
- JSON yolunun ayar dosyasından alınması
- 24–48 saatlik tamamlanmış soak test raporu

## 7/24 kullanım için sonraki geliştirmeler

- Ticker Service bağlantısı için kontrollü reconnect
- Bağlantı kopmalarında artan bekleme
- Son başarılı güncelleme zamanının arayüzde gösterilmesi
- Hata ve işlem loglarının günlük dosyalara yazılması
- Log dosyalarının boyut veya gün bazında döndürülmesi
- Uygulama ve servis için otomatik başlangıç
- Watchdog veya Windows Service mimarisi
- Uzun süreli yük ve bağlantı testi
- Gerçek veri kaynağı için güvenli doğrulama
- Birden fazla şehir için kontrollü grup yönetimi

## GitHub notları

Repository’ye aşağıdaki klasörler eklenmemelidir:

```text
.vs/
bin/
obj/
```

Ayrıca COM kütüphanesinin ve Viz kurulumunun hedef bilgisayarda ayrıca bulunması gerekir.

JSON yolu şu anda kullanıcıya özel ve sabittir:

```text
C:\Users\metutun\Documents\viztickerservice secim\canli_test.json
```

Proje başka bilgisayara taşınacaksa bu yol kontrollü bir ayar dosyasına alınmalıdır.

## Mevcut çalışma durumu

Doğrulanan özellikler:

- Yerel Ticker Service bağlantısı çalışıyor.
- JSON değişiklikleri otomatik algılanıyor.
- 400 ms debounce çalışıyor.
- XML otomatik oluşturuluyor.
- Element `UpdateElement` ile güncelleniyor.
- Ticker Service geri okuması çalışıyor.
- TTL süresiz durumda.
- 1/2 ve 3/4 manuel direktör komutları çalışıyor.
- Checkbox ile 5 saniyelik otomatik sayfa geçişi çalışıyor.
- Form kapanırken timer ve Viz Engine bağlantısı temizleniyor.

Bu proje şu anda yerel Ankara test sahnesi için çalışan bir prototiptir. Canlı yayın sistemine geçirilmeden önce uzun süreli test, otomatik reconnect, loglama ve yapılandırma çalışmaları tamamlanmalıdır.
