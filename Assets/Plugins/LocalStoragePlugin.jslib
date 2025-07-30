// LocalStoragePlugin.jslib
mergeInto(LibraryManager.library, {
    // Сохранение данных в Local Storage
    SaveToLocalStorage: function (key, data) {
        // Преобразуем JS-строки обратно в нормальные C# строки
        var jsKey = Pointer_stringify(key);
        var jsData = Pointer_stringify(data);
        
        console.log("Saving to LocalStorage. Key: " + jsKey + ", Data length: " + jsData.length);
        localStorage.setItem(jsKey, jsData);
    },

    // Загрузка данных из Local Storage
    LoadFromLocalStorage: function (key) {
        var jsKey = Pointer_stringify(key);
        var data = localStorage.getItem(jsKey);
        
        console.log("Loading from LocalStorage. Key: " + jsKey + ", Found: " + (data !== null));
        // Если данных нет, возвращаем пустую строку, чтобы избежать ошибок в C#
        if (data === null) {
            return stringToNewUTF8(" "); // Важно вернуть что-то, чтобы C# не получил null
        }
        return stringToNewUTF8(data);
    }
});