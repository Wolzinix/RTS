
using UnityEngine;
public class GPUInstancing : MonoBehaviour
{
    Matrix4x4[] matrice;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    [SerializeField] int NbOfObject;
    RenderParams rp;

    private void Start()
    {
        matrice = new Matrix4x4[NbOfObject];
        rp = new RenderParams(material);
    }
    void Update()
    {
        if(matrice.Length != 0)
        {
            Graphics.RenderMeshInstanced(rp, mesh, 0, matrice);
        }
    }

    public void AddInMatrice(Vector3 position ,Quaternion rotation,Vector3 size)
    {
        int i = 0;
        while (i < matrice.Length)
        {
            if (matrice[i] != Matrix4x4.zero)
            {
                matrice[i] = Matrix4x4.TRS(position, rotation, size);
                i = matrice.Length;
            }
            i++;
        }
    }

    public void RemoveFromMatrice(Vector3 position, Quaternion rotation, Vector3 size)
    {
        Matrix4x4 search = Matrix4x4.TRS(position, rotation, size);

        int i = 0;
        while ( i < matrice.Length)
        {
            if(matrice[i] == search)
            {
                matrice[i] = Matrix4x4.zero;
            }
            i++;
        }
    }
}
