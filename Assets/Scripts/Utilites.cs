using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilites : MonoBehaviour
{
    public static T NullCheckFindGameObjectGetComponent <T> (string gameObjectToFindName,T componentToTryGet)
    {
        GameObject genericObject = GameObject.Find(gameObjectToFindName);

        if (genericObject != null)
        {
            if (genericObject.TryGetComponent(out T genericComponentVariable))
            {
                if (componentToTryGet.GetType() == genericComponentVariable.GetType())
                    return genericComponentVariable;
            }
            else
                Debug.LogError("Error could not find "+componentToTryGet.GetType().ToString());
        }
        else
            Debug.LogError("Could not find gameObject named: "+gameObjectToFindName);
        

        return default;
    }
}
