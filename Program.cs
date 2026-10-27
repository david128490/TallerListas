using System.Globalization;
using TallerListas;

// Delegado para convertir el texto que escribe el usuario al tipo T
delegate bool Convertidor<T>(string texto, out T valor);

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== LISTA DOBLEMENTE LIGADA GENÉRICA ===");
        Console.WriteLine("¿Con qué tipo de dato quieres trabajar?");
        Console.WriteLine("1. Números enteros (int)");
        Console.WriteLine("2. Texto (string)");
        Console.WriteLine("3. Números decimales (double)");
        Console.Write("Opción: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                EjecutarMenu<int>(int.TryParse);
                break;
            case "2":
                EjecutarMenu<string>((string t, out string v) =>
                {
                    v = t.Trim();
                    return v.Length > 0;
                });
                break;
            case "3":
                EjecutarMenu<double>(double.TryParse);
                break;
            default:
                Console.WriteLine("Opción no válida.");
                break;
        }
    }

    static void EjecutarMenu<T>(Convertidor<T> convertir) where T : IComparable<T>
    {
        var lista = new ListaDoblementeLigada<T>();
        bool salir = false;

        while (!salir)
        {
            MostrarMenu();
            string? opcion = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (opcion)
            {
                case "1": // Adicionar
                    if (LeerValor("Valor a adicionar: ", convertir, out T nuevo))
                    {
                        lista.Adicionar(nuevo);
                        Console.WriteLine($"Se adicionó {nuevo}.");
                        Console.WriteLine($"Lista: {lista.MostrarAdelante()}");
                    }
                    break;

                case "2": // Mostrar hacia adelante
                    if (HayElementos(lista))
                        Console.WriteLine($"Hacia adelante: {lista.MostrarAdelante()}");
                    break;

                case "3": // Mostrar hacia atrás
                    if (HayElementos(lista))
                        Console.WriteLine($"Hacia atrás: {lista.MostrarAtras()}");
                    break;

                case "4": // Ordenar descendentemente
                    if (HayElementos(lista))
                    {
                        if (lista.OrdenarDescendentemente())
                            Console.WriteLine($"Lista ordenada descendentemente: {lista.MostrarAdelante()}");
                        else
                            Console.WriteLine("La lista ya está ordenada descendentemente.");
                    }
                    break;

                case "5": // Moda(s)
                    if (HayElementos(lista))
                        MostrarModas(lista);
                    break;

                case "6": // Gráfico
                    if (HayElementos(lista))
                        MostrarGrafico(lista);
                    break;

                case "7": // Existe
                    if (LeerValor("Valor a buscar: ", convertir, out T buscado))
                    {
                        if (lista.Existe(buscado))
                            Console.WriteLine($"Sí existe el {buscado} en la lista.");
                        else
                            Console.WriteLine($"No existe el {buscado}.");
                    }
                    break;

                case "8": // Eliminar una ocurrencia
                    if (LeerValor("Valor a eliminar (una ocurrencia): ", convertir, out T unico))
                    {
                        if (lista.EliminarUna(unico))
                            Console.WriteLine($"Se eliminó una ocurrencia del {unico}.");
                        else
                            Console.WriteLine($"No existe el {unico}, no se eliminó nada.");
                    }
                    break;

                case "9": // Eliminar todas las ocurrencias
                    if (LeerValor("Valor a eliminar (todas las ocurrencias): ", convertir, out T todos))
                    {
                        int cantidad = lista.EliminarTodas(todos);
                        if (cantidad > 0)
                            Console.WriteLine($"Se eliminaron {cantidad} ocurrencia(s) del {todos}.");
                        else
                            Console.WriteLine($"No existe el {todos}, no se eliminó nada.");
                    }
                    break;

                case "0":
                    salir = true;
                    Console.WriteLine("¡Hasta luego!");
                    break;

                default:
                    Console.WriteLine("Opción no válida. Intenta de nuevo.");
                    break;
            }

            if (!salir)
            {
                Console.WriteLine("\nPresiona una tecla para continuar...");
                Console.ReadKey(true);
            }
        }
    }

    static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine("========== MENÚ ==========");
        Console.WriteLine("1. Adicionar");
        Console.WriteLine("2. Mostrar hacia adelante");
        Console.WriteLine("3. Mostrar hacia atrás");
        Console.WriteLine("4. Ordenar descendentemente");
        Console.WriteLine("5. Mostrar la(s) moda(s)");
        Console.WriteLine("6. Mostrar gráfico");
        Console.WriteLine("7. Existe");
        Console.WriteLine("8. Eliminar una ocurrencia");
        Console.WriteLine("9. Eliminar todas las ocurrencias");
        Console.WriteLine("0. Salir");
        Console.WriteLine("==========================");
        Console.Write("Opción: ");
    }

    static bool LeerValor<T>(string mensaje, Convertidor<T> convertir, out T valor)
    {
        Console.Write(mensaje);
        string? texto = Console.ReadLine();

        if (texto != null && convertir(texto, out valor))
            return true;

        Console.WriteLine("Valor no válido para el tipo de dato elegido.");
        valor = default!;
        return false;
    }

    static bool HayElementos<T>(ListaDoblementeLigada<T> lista) where T : IComparable<T>
    {
        if (lista.EstaVacia)
        {
            Console.WriteLine("La lista está vacía.");
            return false;
        }
        return true;
    }

    static void MostrarModas<T>(ListaDoblementeLigada<T> lista) where T : IComparable<T>
    {
        var (modas, frecuencia) = lista.ObtenerModas();

        if (modas.Count == 1)
        {
            Console.WriteLine($"La moda es {modas[0]}. (aparece {frecuencia} vez/veces)");
            return;
        }

        string texto = string.Join(", ", modas.Take(modas.Count - 1)) + " y " + modas[^1];
        Console.WriteLine($"Las modas son {texto}. (cada una aparece {frecuencia} vez/veces)");
    }

    static void MostrarGrafico<T>(ListaDoblementeLigada<T> lista) where T : IComparable<T>
    {
        Console.WriteLine("Ocurrencias:");
        foreach (var (valor, veces) in lista.ObtenerOcurrencias())
            Console.WriteLine($"{valor,-10} {new string('*', veces)}");
    }
}
