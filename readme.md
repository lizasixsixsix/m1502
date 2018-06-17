# 15 serialization

## 02 custom serialization

### общее

В данном задании мы пытаемся сериализовать с помощью `DataContractSerializer`
результат выборки различных данных из базы _Northwind_
(для удобства можно использовать готовый solution _Task_,
который представлен в материалах).

Проблема, которую мы пытаемся решить, состоит в следующем:
ОРМ библиотеки, использующие механизмы _Lazy Loading_
(такие как _Entity Framework_ или _NHibernate_),
как правило, делают это, используя автогенерируемые прокси-классы,
о которых сериализатор ничего не знает.

В результате мы получаем примерно такое сообщение:

```
System.Runtime.Serialization.SerializationException: Type 'System.Data.Entity.DynamicProxies.Category_C0BBA388964A08FA890D2324D05497A5E29614C817E8716A99529D02E4D2DC8A' with data contract name 'Category_C0BBA388964A08FA890D2324D05497A5E29614C817E8716A99529D02E4D2DC8A:http://schemas.datacontract.org/2004/07/System.Data.Entity.DynamicProxies' is not expected. Consider using a DataContractResolver if you are using DataContractSerializer or add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to the serializer.
```

Здесь возможны разные варианты решения:

*   Отказ от ленивой загрузки.

    Если установить для EF настройку
	`dbContext.Configuration.ProxyCreationEnabled = false;`,
    то он перестанет использовать _proxy_-классы,
    но при этом все связанные сущности перестанут загружаться
    (например, у объекта `Category` есть свойство `Products`,
    которое при отключении генерации _Proxy_ будет содержать коллекцию
    с 0 элементов).

*   Предварительное получение имён всех подобных _Proxy_ классов
    и передача их в _KnownTypes_.

*   Доработка самого процесса сериализации.

**Примечание!!! Если вам более знаком другой ORM с поддержкой _Lazy Loading_
(например, )NHibernate_), вы можете решать данную задачу с помощью него.
Но прежде нужно убедиться, что проблема воспроизводится.**

### задание

В solution _Task_ (который можно найти в материалах) имеется тестовый класс
`SerializationSolutions`, каждый метод которого пытается загрузить
и сериализовать список сущностей того или иного типа.

При этом:
*   Часть методов использует генерацию _proxy_
    (и получают ошибку сериализации), а часть &mdash; нет
    (и не загружают связанные сущности).
*   Часть сериализует с помощью `DataContractSerializer`, а часть &mdash;
    `NetDataContractSerializer` (он выбран потому, что лучше поддерживает
    такие методы сериализации, как `ISerializationSurrogate`
    и _Serialization Callbacks_ &mdash; например, он позволяет
    передавать `StreamingContext` в _callbacks_,
    чего не позволяет `DataContractSerializer`).

Используя тот способ расширения процесса сериализации,
который указан в имени метода, добейтесь корректной работы каждого метода,
а именно:
1.	Сериализация и десериализация работают без ошибок.
1.	Сериализуется сами сущности, а не _proxy_.
1.	Сериализуется основная сущность (та, которую мы запрашиваем)
    и связанные с нею (но только на 1 уровень вложенности!).

**Примечание!!! Тем, кто мало работал с _Entity Framework_, напоминаю,
что если у сущности есть связи, но при этом отключен _LazyLoading_,
поле со связанной сущностью можно проинициализировать так
(например, для конкретной категории загрузить список продуктов):**

```csharp
var t = (dbContext as IObjectContextAdapter).ObjectContext;
t.LoadProperty(category, f => f.Products);
```

**Только не забывайте, что `DbContext` должен быть тем же самым,
в котором была получена сама категория!**
