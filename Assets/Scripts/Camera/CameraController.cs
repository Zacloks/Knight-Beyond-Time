using UnityEngine;

public class CameraController : MonoBehaviour
{
   [Tooltip("Opcional. Si se deja vacío, la cámara busca sola al objeto con tag 'Player' (que ahora se instancia en runtime).")]
   public Transform objetivo;
   public string targetTag = "Player";
   public float smoothing = 0.125f; // Qué tan rápido te sigue (ej: 0.125)
   public Vector3 desplazamiento;

   public float minX; // borde izquierdo
   public float maxX; //borde derecho
   public float minY; // borde inferior
   public float maxY; //borde superior

   private float baseMinX, baseMaxX;

   private void Awake()
   {
      baseMinX = minX;
      baseMaxX = maxX;
   }

   public void SetHorizontalLimits(float left, float right)
   {
      minX = left;
      maxX = right;
   }

   public void ResetHorizontalLimits()
   {
      minX = baseMinX;
      maxX = baseMaxX;
   }

   private void LateUpdate(){
      if (objetivo == null)
      {
         GameObject player = GameObject.FindGameObjectWithTag(targetTag);
         if (player != null) objetivo = player.transform;
      }
      if (objetivo == null) return;

        // 1. Calculamos la posición a la que desearía ir la cámara (incluyendo offset)
        Vector3 posicionDeseada = objetivo.position + desplazamiento;

        // 2. Limitamos esa posición para que nunca se pase de los bordes del mapa
        // El Mathf.Clamp es lo que evita que se vea el vacío.
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        float leftLim = minX + camWidth;
        float rightLim = maxX - camWidth;
        float xLimitada = (leftLim <= rightLim)
            ? Mathf.Clamp(posicionDeseada.x, leftLim, rightLim)
            : (minX + maxX) * 0.5f;

        float yLimitada = Mathf.Clamp(
            posicionDeseada.y,
            minY + camHeight,
            maxY - camHeight
        );

        // 3. Creamos el vector de destino final, manteniendo la profundidad Z original de la cámara (ej: -10)
        Vector3 posicionConLimites = new Vector3(xLimitada, yLimitada, transform.position.z);

        // 4. Movemos la cámara suavemente hacia la posición limitada usando Lerp
        // Usamos Time.deltaTime para que el suavizado sea constante sin importar los FPS.
        transform.position = Vector3.Lerp(transform.position, posicionConLimites, smoothing);

   }
   private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY));
        Gizmos.DrawLine(new Vector2(maxX, minY), new Vector2(maxX, maxY));
        Gizmos.DrawLine(new Vector2(maxX, maxY), new Vector2(minX, maxY));
        Gizmos.DrawLine(new Vector2(minX, maxY), new Vector2(minX, minY));
    }
}
