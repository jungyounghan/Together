using System;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[DisallowMultipleComponent]
/// <summary>
/// Hashtable이 제대로 전달되었는지 확인하는 클래스이다.
/// </summary>
public class Tester : MonoBehaviour
{
#if UNITY_EDITOR
    [Serializable]
    private struct Data
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private string value;

        public string getKey {
            get
            {
                return key;
            }
        }

        public string getValue {
            get
            {
                return value;
            }
        }

        public Data(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    [SerializeField]
    private List<Data> _dataList = new List<Data>();

    public void Set(Hashtable hashtable)
    {
        if (hashtable != null)
        {
            _dataList.Clear();
            foreach (string key in hashtable.Keys)
            {
                string value = hashtable[key] != null ? hashtable[key].ToString() : null;
                _dataList.Add(new Data(key, value));
            }
        }
    }
#endif
}