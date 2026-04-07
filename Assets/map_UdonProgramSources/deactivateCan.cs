using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class deactivateCan : UdonSharpBehaviour
{
    // Layer de la papelera para filtrar qué objetos activan el trigger
    public int trashLayer = 25; // Cambia al número de capa de tu papelera

    void OnTriggerEnter(Collider other)
    {
        // Verifica que el trigger sea la papelera
        if (other.gameObject.layer == trashLayer)
        {
            // Desactiva este objeto (la lata)
            gameObject.SetActive(false);
            gameObject.transform.position = new Vector3(0, 0, 0); // Mueve la lata a una posición fuera de la vista
        }
    }
}