using UnityEngine;

public class ProximityDither : MonoBehaviour
{
    [Tooltip("Check this if renderer is using dither for alpha clipping, otherwise the object will be disabled at distanceMax")]
    public bool ditherActive = true;
    public float distanceMin = 0.25f;
    public float distanceMax = 0.5f;

    void OnValidate()
    {
        if( distanceMin < 0 ) distanceMin = 0;
        if( distanceMax < 0 ) distanceMax = 0;

        if( distanceMin > distanceMax ) distanceMax = distanceMin + 0.1f;
        if( distanceMax < distanceMin ) distanceMin = distanceMax - 0.1f;
    }

    public Renderer renderToHide;

    [Tooltip("If true the gameObject active state will be set to false instead of changing the renderToHide.enable value")]
    public bool disableRenderGO = false;

    [Header("Optional")]
    [Tooltip("Keep value as null to auto assign Main camera transform as the target value")]
    public Transform target;

    Material renderMaterial;

    public void Awake()
    {
        if( target == null ) target = Camera.main; 

        if( renderToHide == null ) Debug.LogError("Null render reference");

        renderMaterial = renderToHide.material;
    }

    void SetRenderActive( bool value )
    {
        if( disableRenderGO ) renderToHide.gameObject.SetActive( value );

        else renderToHide.enabled = value;
    }

    static readonly int PDistMin = Shader.PropertyToID("DistanceMin");
    static readonly int PDistMax = Shader.PropertyToID("DistanceMax");

    public void Update()
    {
        var dist = Vector3.Distance( transform.position , target.position );

        dist = ( dist - distanceMin ) / ( distanceMax - distanceMin );

        // Draw.label( transform.position, dist.ToString("N2") , Color.red, 0.025f );

        bool active = disableRenderGO ? renderToHide.gameObject.activeSelf : renderToHide.enabled;

        if( ! ditherActive )
        {
            if( dist < 1 && active )
            {
                SetRenderActive( false );
            }
            if( dist > 1 && ! active )
            {
                SetRenderActive( true );
            }
        }
        else 
        {
            if( dist < 0 && active )
            {
                SetRenderActive( false );
            }
            if( dist > 0 && ! active )
            {
                SetRenderActive( true );
            }

            // you can avoid using everything else in this script and only set the shader varialbes instead 
            // if you don't wish to enable / distable objects based on distance 

            renderMaterial.SetFloat( PDistMin, distanceMin );
            renderMaterial.SetFloat( PDistMax, distanceMax );
        }
    }
}
