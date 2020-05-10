using System;
using System.Collections.Generic;

namespace SQLTrainApp.Model.Logic
{
    public static class Mediator
    {
        private static IDictionary<string, List<Action<object>>> pageDictionary =
           new Dictionary<string, List<Action<object>>>();

        public static void Append(string uiName, Action<object> action)
        {
            if (!pageDictionary.ContainsKey(uiName))
            {
                var list = new List<Action<object>>();
                list.Add(action);
                pageDictionary.Add(uiName, list);
            }
            else
            {
                bool contains = false;
                foreach (var item in pageDictionary[uiName])
                    if (item.Method.ToString() == action.Method.ToString())
                        contains = true;
                if (!contains)
                    pageDictionary[uiName].Add(action);
            }
        }

        public static void Finish(string uiName, Action<object> action)
        {
            if (pageDictionary.ContainsKey(uiName))
                pageDictionary[uiName].Remove(action);
        }

        public static void Inform(string uiName, object args = null)
        {
            if (pageDictionary.ContainsKey(uiName))
                foreach (var action in pageDictionary[uiName])
                    action(args);
        }
    }
}
