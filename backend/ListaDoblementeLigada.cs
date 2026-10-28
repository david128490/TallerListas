using System.Text;

namespace TallerListas;

/// <summary>
/// Lista doblemente ligada genérica que mantiene sus elementos ordenados
/// al insertarlos (ascendente por defecto). Acepta elementos repetidos.
/// </summary>
public class ListaDoblementeLigada<T> where T : IComparable<T>
{
    private Nodo<T>? _cabeza;
    private Nodo<T>? _cola;

    // true = la lista está en orden ascendente, false = descendente.
    // Las inserciones respetan el orden actual de la lista.
    private bool _ascendente = true;

    public int Cantidad { get; private set; }
    public bool EstaVacia => Cantidad == 0;

    // Compara según el orden actual de la lista.
    private int Comparar(T a, T b) => _ascendente ? a.CompareTo(b) : b.CompareTo(a);

    // ---------------------------------------------------------------
    // 1. Adicionar (inserción directa en la posición correcta)
    // ---------------------------------------------------------------
    public void Adicionar(T dato)
    {
        var nuevo = new Nodo<T>(dato);

        if (_cabeza == null)
        {
            _cabeza = _cola = nuevo;
        }
        else
        {
            // Se busca el primer nodo que deba quedar DESPUÉS del nuevo.
            // Con ">= 0" los repetidos quedan juntos y el nuevo va al final del grupo.
            Nodo<T>? actual = _cabeza;
            while (actual != null && Comparar(dato, actual.Dato) >= 0)
                actual = actual.Siguiente;

            if (actual == null)
            {
                // Va al final
                nuevo.Anterior = _cola;
                _cola!.Siguiente = nuevo;
                _cola = nuevo;
            }
            else if (actual.Anterior == null)
            {
                // Va al inicio
                nuevo.Siguiente = _cabeza;
                _cabeza!.Anterior = nuevo;
                _cabeza = nuevo;
            }
            else
            {
                // Va en medio, antes de 'actual'
                nuevo.Anterior = actual.Anterior;
                nuevo.Siguiente = actual;
                actual.Anterior.Siguiente = nuevo;
                actual.Anterior = nuevo;
            }
        }

        Cantidad++;
    }

    // ---------------------------------------------------------------
    // 2. Mostrar hacia adelante
    // ---------------------------------------------------------------
    public string MostrarAdelante()
    {
        var sb = new StringBuilder();
        for (var n = _cabeza; n != null; n = n.Siguiente)
        {
            if (sb.Length > 0) sb.Append(", ");
            sb.Append(n.Dato);
        }
        return sb.ToString();
    }

    // ---------------------------------------------------------------
    // 3. Mostrar hacia atrás
    // ---------------------------------------------------------------
    public string MostrarAtras()
    {
        var sb = new StringBuilder();
        for (var n = _cola; n != null; n = n.Anterior)
        {
            if (sb.Length > 0) sb.Append(", ");
            sb.Append(n.Dato);
        }
        return sb.ToString();
    }

    // ---------------------------------------------------------------
    // 4. Ordenar descendentemente (cambia realmente el orden de la lista
    //    invirtiendo los enlaces de cada nodo, no solo la forma de mostrarla)
    // ---------------------------------------------------------------
    /// <returns>false si la lista ya estaba en orden descendente.</returns>
    public bool OrdenarDescendentemente()
    {
        if (!_ascendente) return false;

        var actual = _cabeza;
        while (actual != null)
        {
            var siguiente = actual.Siguiente;
            actual.Siguiente = actual.Anterior;
            actual.Anterior = siguiente;
            actual = siguiente;
        }

        (_cabeza, _cola) = (_cola, _cabeza);
        _ascendente = false;
        return true;
    }

    // ---------------------------------------------------------------
    // Ocurrencias: como la lista siempre está ordenada, los elementos
    // iguales son contiguos. Un solo recorrido basta.
    // ---------------------------------------------------------------
    public List<(T Valor, int Veces)> ObtenerOcurrencias()
    {
        var resultado = new List<(T, int)>();
        var actual = _cabeza;

        while (actual != null)
        {
            T valor = actual.Dato;
            int veces = 0;

            while (actual != null && actual.Dato.CompareTo(valor) == 0)
            {
                veces++;
                actual = actual.Siguiente;
            }

            resultado.Add((valor, veces));
        }

        return resultado;
    }

    // ---------------------------------------------------------------
    // 5. Moda(s)
    // ---------------------------------------------------------------
    public (List<T> Modas, int Frecuencia) ObtenerModas()
    {
        var modas = new List<T>();
        int max = 0;

        foreach (var (valor, veces) in ObtenerOcurrencias())
        {
            if (veces > max)
            {
                max = veces;
                modas.Clear();
                modas.Add(valor);
            }
            else if (veces == max)
            {
                modas.Add(valor);
            }
        }

        return (modas, max);
    }

    // ---------------------------------------------------------------
    // 7. Existe
    // ---------------------------------------------------------------
    public bool Existe(T dato) => Buscar(dato) != null;

    private Nodo<T>? Buscar(T dato)
    {
        for (var n = _cabeza; n != null; n = n.Siguiente)
        {
            if (n.Dato.CompareTo(dato) == 0)
                return n;
        }
        return null;
    }

    // ---------------------------------------------------------------
    // 8. Eliminar una ocurrencia (la primera encontrada)
    // ---------------------------------------------------------------
    public bool EliminarUna(T dato)
    {
        var nodo = Buscar(dato);
        if (nodo == null) return false;

        Desenlazar(nodo);
        return true;
    }

    // ---------------------------------------------------------------
    // 9. Eliminar todas las ocurrencias
    // ---------------------------------------------------------------
    /// <returns>Cantidad de elementos eliminados.</returns>
    public int EliminarTodas(T dato)
    {
        int eliminados = 0;
        var actual = _cabeza;

        while (actual != null)
        {
            var siguiente = actual.Siguiente;
            if (actual.Dato.CompareTo(dato) == 0)
            {
                Desenlazar(actual);
                eliminados++;
            }
            actual = siguiente;
        }

        return eliminados;
    }

    // Saca un nodo de la lista actualizando los enlaces vecinos.
    private void Desenlazar(Nodo<T> nodo)
    {
        if (nodo.Anterior != null)
            nodo.Anterior.Siguiente = nodo.Siguiente;
        else
            _cabeza = nodo.Siguiente;

        if (nodo.Siguiente != null)
            nodo.Siguiente.Anterior = nodo.Anterior;
        else
            _cola = nodo.Anterior;

        nodo.Siguiente = null;
        nodo.Anterior = null;
        Cantidad--;
    }
}