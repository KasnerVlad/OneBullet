using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Custom
{
    public static class Helper
    {
        public static void ClampAngle(this float angle)
        {
            if (angle < -360f) angle += 720f;
            if (angle > 360f) angle -= 720f;
        }

        public static Vector3 Round(this Vector3 pos)
        {
            return new Vector3(pos.x.Round(), pos.y.Round(), pos.z.Round());
        }
        public static float Round(this float pos)
        {
            return Mathf.Round(pos);
        }

        public static bool Approximately(Vector3 a, Vector3 b)
        {
            return Mathf.Approximately(a.x, b.x)&&Mathf.Approximately(a.y, b.y)&&Mathf.Approximately(a.z, b.z);
        }
        /*#if UNITY_EDITOR
        private static int _dataBaseRefreshRequestCount;
        public static void DataBaseRefreshRequest(int _refreshTime) { 
            _dataBaseRefreshRequestCount++;
            if (_dataBaseRefreshRequestCount == 1) {
                _=CustomInvoke.Invoke(()=>{AssetDatabase.Refresh(); _dataBaseRefreshRequestCount=0; },_refreshTime);
            }
        }
        #endif
        public static string GetSlash(string path)
        {
            string slash = "";
            if (path.ToCharArray()[path.ToCharArray().Length - 1] != '/') { slash = "/"; }
            return slash;
        }
        public static List<Transform> FindAllByName(Transform root, string name) {
            List<Transform> results = new List<Transform>();
            foreach (Transform child in root) {
                if (child.name == name) results.Add(child);
                results.AddRange(FindAllByName(child, name));
            }
            return results;
        } public static string GetRelativePath(Transform root, Transform target) {
            List<string> path = new List<string>();
            while (target != root && target != null) {
                path.Insert(0, target.name);
                target = target.parent;
            }
            return string.Join("/", path);
        }*/
        public static Quaternion RotateTo(Vector3 to, Vector3 pos, Vector3 offset)
        {
            Vector3 direction = to - pos;
        
            float angleRad = Mathf.Atan2(direction.y, direction.x);
            float angleDeg = angleRad * Mathf.Rad2Deg;
        
            Quaternion finalRotation = Quaternion.Euler(new Vector3(0f, 0f, angleDeg) + offset);
            
            return finalRotation;
        }

        public static float GetFloatFromInputField(TMP_InputField inputField)
        {
            if (inputField == null || string.IsNullOrEmpty(inputField.text))
            {
                return 0.0f; // Возвращаем 0, если InputField пуст или не задан
            }

            string rawText = inputField.text;
            StringBuilder cleanedTextBuilder = new StringBuilder();
            bool hasDecimalPoint = false;
            bool isFirstChar = true;

            foreach (char c in rawText)
            {
                if (char.IsDigit(c)) // Если символ - цифра
                {
                    cleanedTextBuilder.Append(c);
                }
                else if (c == '.' || c == ',') // Если символ - точка или запятая (десятичный разделитель)
                {
                    // Убедимся, что это первый десятичный разделитель
                    if (!hasDecimalPoint)
                    {
                        // В C# float.Parse ожидает точку как десятичный разделитель по умолчанию для InvariantCulture
                        // или для текущей региональной культуры. Лучше всегда преобразовывать к точке.
                        cleanedTextBuilder.Append('.'); 
                        hasDecimalPoint = true;
                    }
                    // Игнорируем все последующие точки/запятые
                }
                else if (c == '-' && isFirstChar) // Если символ - минус и это первый символ
                {
                    cleanedTextBuilder.Append(c);
                }
                // Игнорируем все остальные символы
                
                isFirstChar = false; // После первого символа, флаг сбрасывается
            }

            string cleanedText = cleanedTextBuilder.ToString();

            // Проверяем, если строка стала пустой после чистки (например, было "abc")
            if (string.IsNullOrEmpty(cleanedText) || cleanedText == "-")
            {
                return 0.0f; // Возвращаем 0, если не осталось числовых символов
            }

            float result;
            // Используем TryParse для безопасного преобразования
            // NumberStyles.Float позволяет парсить числа с десятичной точкой и знаком
            // CultureInfo.InvariantCulture гарантирует, что точка всегда будет разделителем, независимо от региональных настроек пользователя
            if (float.TryParse(cleanedText, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                inputField.text = result.ToString(CultureInfo.InvariantCulture);
                return result;
            }
            else
            {
                // Это может произойти, если, например, строка стала "." или "-" после чистки
                Debug.LogWarning($"Не удалось преобразовать очищенную строку '{cleanedText}' в float. Возвращаем 0.");
                return 0.0f;
            }
        }
    }
    public static class LayerMaskComparer
    {
        public static bool Equals(int storedMask, int queryMask)
        {
            // Точное совпадение - высший приоритет
            if (storedMask == queryMask) return true;
        
            // Проверка частичного совпадения

            return ((1 << queryMask) & storedMask) != 0;
        }
    }
}