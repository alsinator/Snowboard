using UnityEngine;
using System.Collections.Generic;

namespace Snowboard
{
    public class ObjectsPool : MonoBehaviour
    {
        public int InitalInstances = 10;

        [SerializeField] PoolObject[] objectsToPool;

        [SerializeField] Transform objectsParent;

        private Dictionary<string, List<PoolObject>> pool;


        public void Init()
        {

            pool = new Dictionary<string, List<PoolObject>>();

            for (int i = 0; i < objectsToPool.Length; i++)
            {
                var objList = new List<PoolObject>();
                pool.Add(objectsToPool[i].Id, objList);

                for (int j = 0; j < InitalInstances; j++)
                {
                    AddNewInstance(objectsToPool[i]);
                }
            }
        }


        public PoolObject GetObject(string Id)
        {
            var objectsList = new List<PoolObject>();
            if (pool.TryGetValue(Id, out objectsList))
            {
                for (int i = 0; i < objectsList.Count; i++)
                {
                    var obj = objectsList[i];
                    if (obj.Pooled)
                    {
                        obj.Pooled = false;
                        return obj;
                    }
                }

                var newObj = AddNewInstance(objectsList[0]);
                newObj.Pooled = false;
                return newObj;
            }

            return null;
        }


        private PoolObject AddNewInstance(PoolObject poolObj)
        {
            var newInstance = Instantiate(poolObj);

            newInstance.transform.SetParent(objectsParent);
            newInstance.PoolParent = objectsParent;
            newInstance.Pooled = true;
            newInstance.gameObject.SetActive(false);
            pool[poolObj.Id].Add(newInstance);

            return newInstance;
        }
    }
}