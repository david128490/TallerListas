namespace TallerListas;

/// <summary>
/// Nodo de una lista doblemente ligada genérica.
/// </summary>
public class Nodo<T>
{
    public T Dato { get; set; }
    public Nodo<T>? Siguiente { get; set; }
    public Nodo<T>? Anterior { get; set; }

    public Nodo(T dato)
    {
        Dato = dato;
    }
}