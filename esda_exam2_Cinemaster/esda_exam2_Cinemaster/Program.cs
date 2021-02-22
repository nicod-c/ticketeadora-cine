using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esda_exam2_Cinemaster
{

    class Program
    {
        enum EstadoAsiento
        {
            Libre,
            Ocupado,
        }

        class Persona
        {
            public string Nombre;
            public string Apellido;

            public Persona(string nom, string ape)
            {
                this.Nombre = nom;
                this.Apellido = ape;
            }
        }

        class Pelicula
        {
            public string Titulo;
            public string TituloOrignal;
            public Persona Director;
            public Dictionary<Persona, string> Reparto = new Dictionary<Persona, string>();
            public TimeSpan Duracion = new TimeSpan();
            public string Sinopsis;

            public Pelicula(string tit, string toriginal, Persona dir, Dictionary<Persona, string> rep, TimeSpan dur, string sinop)
            {
                this.Titulo = tit;
                this.TituloOrignal = toriginal;
                this.Director = dir;
                this.Reparto = rep;
                this.Duracion = dur;
                this.Sinopsis = sinop;
            }
        }

        class Asiento
        {
            public int Fila;
            public int Columna;
            public bool EsVip = false;

            public Asiento(int f, int c)
            {
                this.Fila = f;
                this.Columna = c;
            }
        }

        class Sala
        {
            public int Numero;
            public Asiento[,] Asientos;

            public Sala(int num, int fil, int col)
            {
                this.Numero = num;
                this.Asientos = new Asiento[fil, col];

                for (int f = 0; f < Asientos.GetLength(0); f++)
                {
                    for (int c = 0; c < Asientos.GetLength(1); c++)
                    {
                        Asientos[f, c] = new Asiento(f, c);
                        if (f == Asientos.GetLength(0) - 1)
                        {
                            Asientos[f, c].EsVip = true;
                        }
                    }
                }
            }
        }

        class Funcion
        {
            public Pelicula Pelicula;
            public Sala Sala;
            public DateTime FechaHora;
            public Dictionary<Asiento, EstadoAsiento> EstadosAsientos = new Dictionary<Asiento, EstadoAsiento>();

            public Funcion(Pelicula p, Sala s, DateTime f)
            {
                this.Pelicula = p;
                this.Sala = s;
                this.FechaHora = f;

                foreach (Asiento a in Sala.Asientos)
                {
                    this.EstadosAsientos.Add(a, EstadoAsiento.Libre);
                }
            }
        }

        class Entrada
        {
            public Funcion Funcion;
            public Asiento Asiento;
            public double Precio;
            public DateTime FechaEmision = DateTime.Now;

            public Entrada(Funcion f, Asiento a)
            {
                this.Funcion = f;
                this.Asiento = a;
            }

        }

        class Cine
        {
            public string Nombre;
            public List<Pelicula> Peliculas = new List<Pelicula>();
            public List<Sala> Salas = new List<Sala>();
            public List<Funcion> Funciones = new List<Funcion>();
            public double PrecioEntrada;
            public double PrecioEntradaVip;

            public Cine(string nom, double p, double pv)
            {
                this.Nombre = nom;
                this.PrecioEntrada = p;
                this.PrecioEntradaVip = pv;
            }
        }

        static void OcuparAsiento(Asiento a, Funcion funcion)
        {
            funcion.EstadosAsientos[a] = EstadoAsiento.Ocupado;
        }

        static void CopiarPeliEntreListasF(List<Funcion> origen, List<Funcion> destino, Pelicula peli)
        {
            for (int i = 0; i < origen.Count; i++)
            {
                if (origen[i].Pelicula == peli)
                {
                    destino.Add((origen[i]));
                }
            }
        }

        static bool EsFuncionDisponible(Cine cine, Pelicula peli)
        {
            foreach (Funcion f in cine.Funciones)
            {
                if (f.Pelicula != peli && !f.EstadosAsientos.ContainsValue(EstadoAsiento.Libre))
                {
                    return false;
                }
            }

            return true;
        }

        static List<Funcion> Cine_BuscarFuncion(Pelicula peli, Cine cine)
        {
            List<Funcion> funcionesPeli = new List<Funcion>();

            if (peli == null)
            {
                throw new ArgumentNullException("pelicula");
            }

            if (EsFuncionDisponible(cine, peli))
            {
                CopiarPeliEntreListasF(cine.Funciones, funcionesPeli, peli);
            }

            return funcionesPeli;
        }

        static int PedirValorInt(string mensaje)
        {
            int rv = 0;

            Console.Write(mensaje);
            while (!int.TryParse(Console.ReadLine(), out rv))
            {
                Console.WriteLine("Valor invalido.");
                Console.Write(mensaje);
            }
            return rv;
        }

        static int PedirValorIntClampeado(string mensaje, int cotaInferior, int cotaSuperior)
        {
            int rv = PedirValorInt(mensaje);

            while (rv < cotaInferior || rv > cotaSuperior)
            {
                Console.WriteLine("Valor invalido.");
                rv = PedirValorInt(mensaje);
            }

            return rv;
        }

        static string CrearRepeticion(char unChar, int cant)
        {
            string rv = "";

            for (int i = 0; i < cant; i++)
                rv += unChar;

            return rv;
        }

        static void LimpiarPantalla()
        {
            Console.Clear();
        }

        static void ImprimirPantalla(Sala sala)
        {
            int anchoAsientos = sala.Asientos.GetLength(1) * 3;
            char igual = '=';
            string pantalla = "PANTALLA";
            int totalIguales = anchoAsientos - pantalla.Length;
            int lado = totalIguales / 2;

            Console.Write(CrearRepeticion(' ', 2));

            if (totalIguales % 2 != 0)
            {
                int ladoMayor = lado + totalIguales % 2;
                int ladoMenor = totalIguales - ladoMayor;
                Console.WriteLine($"{CrearRepeticion(igual, ladoMenor)}{pantalla}{CrearRepeticion(igual, ladoMayor)}");
            }

            else
                Console.WriteLine($"{CrearRepeticion(igual, lado)}{pantalla}{CrearRepeticion(igual, lado)}");
        }

        static bool EsAsientoLibre(Asiento asiento, Funcion funcion)
        {
            if (funcion.EstadosAsientos[asiento] == EstadoAsiento.Ocupado)
            {
                return false;
            }
            return true;
        }

        static void ImprimirMapaAsientos(Funcion funcion)
        {
            Console.Write(CrearRepeticion(' ', 2));

            for (int x = 0; x < funcion.Sala.Asientos.GetLength(1); x++)
            {
                if (x <= 9)
                {
                    Console.Write($" {x} ");
                }
                else Console.Write($" {x}");

                if (x == funcion.Sala.Asientos.GetLength(1) - 1)
                {
                    Console.WriteLine();
                }
            }

            for (int i = 0; i < funcion.Sala.Asientos.GetLength(0); i++)
            {
                if (i <= 9)
                {
                    Console.Write($"{i} ");
                }
                else Console.Write($"{i}");

                for (int j = 0; j < funcion.Sala.Asientos.GetLength(1); j++)
                {
                    if (!EsAsientoLibre(funcion.Sala.Asientos[i, j], funcion))
                    {
                        Console.Write("[O]");
                    }
                    else if (funcion.Sala.Asientos[i, j].EsVip == true)
                    {
                        Console.Write("[V]");
                    }
                    else Console.Write("[ ]");
                }

                Console.WriteLine();
            }
        }

        static bool EsDiaPromo(Funcion funcion)
        {
            if (funcion.FechaHora.DayOfWeek == DayOfWeek.Wednesday || funcion.FechaHora.DayOfWeek == DayOfWeek.Thursday)
            {
                return true;
            }

            return false;
        }

        static double CalcularPrecioEntrada(Cine cine, Funcion funcion)
        {
            double precio = cine.PrecioEntrada;

            if (EsDiaPromo(funcion))
            {
                precio = cine.PrecioEntrada / 2;
            }
            return precio;
        }

        static double CalcularPrecioEntradaVip(Cine cine, Funcion funcion)
        {
            double precio = cine.PrecioEntradaVip;

            if (EsDiaPromo(funcion))
            {
                precio = cine.PrecioEntradaVip / 2;
            }
            return precio;
        }

        static void MostrarReferenciasFuncion(Cine cine, Funcion funcion)
        {
            double precio = CalcularPrecioEntrada(cine, funcion);
            double precioVip = CalcularPrecioEntradaVip(cine, funcion);
            Console.WriteLine("Referencias: ");

            if (EsDiaPromo(funcion))
            {
                Console.WriteLine($"[ ] = {EstadoAsiento.Libre} ${precio} (precio promo)");
                Console.WriteLine($"[V] = VIP {EstadoAsiento.Libre} ${precioVip} (precio promo)");
            }

            if (!EsDiaPromo(funcion))
            {
                Console.WriteLine($"[ ] = {EstadoAsiento.Libre} ${precio}");
                Console.WriteLine($"[V] = VIP {EstadoAsiento.Libre} ${precioVip}");
            }

            Console.WriteLine($"[O] = {EstadoAsiento.Ocupado}");
        }

        static void MostrarEntrada(Entrada entrada, Cine cine)
        {
            LimpiarPantalla();
            string ticket = "ENTRADA";
            string gracias = "MUCHAS GRACIAS";
            Console.WriteLine(CrearRepeticion('*', cine.Nombre.Length) + cine.Nombre + CrearRepeticion('*', cine.Nombre.Length));
            Console.WriteLine(CrearRepeticion('*', cine.Nombre.Length) + ticket + CrearRepeticion('*', cine.Nombre.Length));
            Console.WriteLine($"Pelicula: {entrada.Funcion.Pelicula.Titulo}");
            Console.WriteLine($"Sala: {entrada.Funcion.Sala.Numero}");
            Console.WriteLine($"Asiento: Fila {entrada.Asiento.Fila} - Butaca {entrada.Asiento.Columna}");
            Console.WriteLine($"Fecha de función: {entrada.Funcion.FechaHora.DayOfWeek} {entrada.Funcion.FechaHora.ToString("dd/MM/yyyy")}");
            Console.WriteLine($"Hora función: {entrada.Funcion.FechaHora.Hour}:{entrada.Funcion.FechaHora.Minute}");
            Console.WriteLine($"Precio: ${entrada.Precio}");
            Console.WriteLine($"Fecha de emisión: {entrada.FechaEmision.ToString("dd/MM/yyyy")}");
            Console.WriteLine(CrearRepeticion('*', cine.Nombre.Length) + gracias + CrearRepeticion('*', cine.Nombre.Length));

            Console.WriteLine("Presione una tecla para continuar...");
            Console.ReadKey();
            LimpiarPantalla();
        }

        static void MostrarInfoFunc(Funcion funcion)
        {
            Console.WriteLine($"Pelicula: {funcion.Pelicula.Titulo}");
            Console.WriteLine($"Funcion: {funcion.FechaHora.ToString("dd/MM/yyyy")}");
        }

        static Entrada CrearEntrada(Funcion funcion, Asiento asiento, Cine cine)
        {
            var entrada = new Entrada(funcion, asiento);
            if (asiento.EsVip == false)
            {
                entrada.Precio = CalcularPrecioEntrada(cine, funcion);
            }

            if (asiento.EsVip == true)
            {
                entrada.Precio = CalcularPrecioEntradaVip(cine, funcion);
            }

            return entrada;
        }

        static bool CancelarEleccion(int eleccion)
        {
            int cancelar = -1;
            if(eleccion == cancelar)
            {
                return true;
            }

            return false;
        }

        static Entrada MostrarPantallaSelecAsiento(Cine cine, Funcion funcion)
        {

            MostrarInfoFunc(funcion);

            ImprimirPantalla(funcion.Sala);

            ImprimirMapaAsientos(funcion);

            MostrarReferenciasFuncion(cine, funcion);

            int f = PedirValorIntClampeado("Ingrese Fila (-1 para cancelar):  ", -1, funcion.Sala.Asientos.GetLength(0) - 1);
            
            while (CancelarEleccion(f))
            {
                LimpiarPantalla();
                Iniciar(cine);
            }

            int c = PedirValorIntClampeado("Ingrese Columna (-1 para cancelar): ", -1, funcion.Sala.Asientos.GetLength(1) - 1);

            while (CancelarEleccion(c))
            {
                LimpiarPantalla();
                Iniciar(cine);
            }

            var asiento = funcion.Sala.Asientos[f, c];

            while (!EsAsientoLibre(asiento, funcion))
            {
                Console.WriteLine("Asiento ocupado.");
                f = PedirValorIntClampeado("Ingrese Fila (-1 para cancelar):  ", -1, funcion.Sala.Asientos.GetLength(0));
                c = PedirValorIntClampeado("Ingrese Columna (-1 para cancelar): ", -1, funcion.Sala.Asientos.GetLength(1));
                asiento = funcion.Sala.Asientos[f, c];
            }

            OcuparAsiento(asiento, funcion);

            return CrearEntrada(funcion, asiento, cine);
        }

        static void ListarFunciones(List<Funcion> funciones)
        {
            Console.WriteLine("Funciones: ");
            for (int i = 0; i < funciones.Count; i++)
            {
                Console.WriteLine($"{i} - {funciones[i].FechaHora.ToString("dd/MM/yyyy")} {funciones[i].FechaHora.ToString("HH:mm")}");
            }

            Console.WriteLine($"{funciones.Count} - Cancelar");
        }

        static int ElegirPelicula(List<Pelicula> peliculas)
        {
            int eleccion = -1;

            eleccion = PedirValorIntClampeado("Su elección: ", 0, peliculas.Count - 1);

            return eleccion;
        }

        static int ElegirFuncion(List<Funcion> funciones)
        {
            int eleccion = -1;

            eleccion = PedirValorIntClampeado("Su elección: ", 0, funciones.Count);

            return eleccion;
        }

        static Funcion MostrarPantallaSelecFuncion(Cine cine, Pelicula peli)
        {
            MostrarInfoPelicula(peli);

            List<Funcion> funciones = Cine_BuscarFuncion(peli, cine);

            ListarFunciones(funciones);

            int eleccion = ElegirFuncion(funciones);

            int cancelar = funciones.Count;

            while (eleccion == cancelar)
            {
                LimpiarPantalla();
                Iniciar(cine);
            }

            Funcion funcionElegida = funciones[eleccion];

            LimpiarPantalla();

            return funcionElegida;
        }

        static void MostrarInfoPelicula(Pelicula peli)
        {
            Console.WriteLine($"Título: {peli.Titulo}");
            Console.WriteLine($"Título original: {peli.TituloOrignal}");
            string output = null;
            output = "Duración: " + peli.Duracion.ToString(@"hh") + " " + "hora" + " " + peli.Duracion.ToString(@"mm") + " " + "minutos";
            Console.WriteLine(output);
            Console.WriteLine($"Director: {peli.Director.Nombre} {peli.Director.Apellido}");
            Console.WriteLine($"Reparto: ");
            foreach (Persona p in peli.Reparto.Keys)
            {
                Console.WriteLine("*" + p.Nombre + " " + " " + p.Apellido + " " + "como" + " " + peli.Reparto[p]);
            }
            Console.WriteLine($"Sinopsis: {peli.Sinopsis}");
        }

        static void ListarPeliculas(Cine cine)
        {
            for (int i = 0; i < cine.Peliculas.Count; i++)
            {
                Console.WriteLine($"{i} - {cine.Peliculas[i].Titulo} ({cine.Peliculas[i].TituloOrignal})");
            }
        }

        static Pelicula PantallaSelecPeliculas(Cine cine)
        {
            ListarPeliculas(cine);

            Pelicula peliculaElegida = cine.Peliculas[ElegirPelicula(cine.Peliculas)];

            LimpiarPantalla();

            return peliculaElegida;
        }

        static void MostrarMsgBienvenida(Cine cine)
        {
            Console.WriteLine($"Bienvenido a {cine.Nombre}.");
            Console.WriteLine("Qué película quieres ver?.");
        }

        static Dictionary<Asiento, EstadoAsiento> CompletarEstadoAsientosFunc(Asiento[,] asientos)
        {
            Dictionary<Asiento, EstadoAsiento> asientosSala = new Dictionary<Asiento, EstadoAsiento>();
            foreach (Asiento a in asientos)
            {
                asientosSala.Add(a, EstadoAsiento.Libre);
            }

            return asientosSala;
        }

        static void Iniciar(Cine cine)
        {
            MostrarMsgBienvenida(cine);
            Pelicula pelicula = PantallaSelecPeliculas(cine);
            Funcion funcion = MostrarPantallaSelecFuncion(cine, pelicula);
            Entrada entrada = MostrarPantallaSelecAsiento(cine, funcion);
            MostrarEntrada(entrada, cine);
        }

        static void Main(string[] args)
        {
            #region Cine
            string nombreUnCine = "CineMaster";
            double precioEntrada = 200;
            double precioEntradavip = 250;
            Cine unCine = new Cine(nombreUnCine, precioEntrada, precioEntradavip);
            #endregion

            #region Salas
            Sala salaUno = new Sala(1, 6, 9);
            Sala salaDos = new Sala(2, 10, 13);
            Sala salaTres = new Sala(3, 5, 8);
            Sala salaCuatro = new Sala(4, 8, 11);

            CompletarEstadoAsientosFunc(salaDos.Asientos);
            CompletarEstadoAsientosFunc(salaTres.Asientos);
            CompletarEstadoAsientosFunc(salaCuatro.Asientos);
            #endregion

            #region Personas
            Dictionary<Persona, string> repartoUno = new Dictionary<Persona, string>();
            Persona directorUno = new Persona("Dean", "Deblois");
            Persona jayBaruchel = new Persona("Jay", "Baruchel");
            Persona americaFerrera = new Persona("America", "Ferrera");
            Persona cameBlanchet = new Persona("Came", "Blanchet");
            Persona fMurraya = new Persona("F. Murray", "Abraham");
            Persona gerardButler = new Persona("Gerard", "Butler");
            repartoUno.Add(jayBaruchel, "Hiccup (voice)");
            repartoUno.Add(americaFerrera, "Astrid (voice)");
            repartoUno.Add(cameBlanchet, "Valka (voice)");
            repartoUno.Add(fMurraya, "Grimmel (voice)");
            repartoUno.Add(gerardButler, "Stoick (voice)");

            Dictionary<Persona, string> repartoDos = new Dictionary<Persona, string>();
            Persona directorDos = new Persona("Leigh", "Whannell");
            Persona elisabethMoss = new Persona("Elisabeth", "Moss");
            Persona oliverCohen = new Persona("Oliver", "Jackson-Cohen");
            repartoDos.Add(elisabethMoss, "Cecilia Cee Kass");
            repartoDos.Add(oliverCohen, "Adrian Griffin");

            Dictionary<Persona, string> repartoTres = new Dictionary<Persona, string>();
            Persona directorTres = new Persona("Lorene", "Scafaria");
            Persona jenifferLo = new Persona("Jeniffer", "Lopez");
            repartoTres.Add(jenifferLo, "Ramona Vega");

            Dictionary<Persona, string> repartoCuatro = new Dictionary<Persona, string>();
            Persona directorCuatro = new Persona("Chris", "Renaud");
            Persona harrisonFord = new Persona("Harrison", "Ford");
            Persona ericStone = new Persona("Eric", "Stonstreet");
            repartoCuatro.Add(harrisonFord, "Rooster (voice)");
            repartoCuatro.Add(ericStone, "Duke (voice)");
            #endregion

            #region Duraciones
            TimeSpan duracionUno = new TimeSpan(01, 44, 00);
            TimeSpan duracionDos = new TimeSpan(02, 01, 00);
            TimeSpan duracionTres = new TimeSpan(01, 29, 00);
            TimeSpan duracionCuatro = new TimeSpan(01, 52, 00);
            #endregion

            #region Peliculas
            string tituloUno = "Cómo entrenar a tu dragón 3";
            string titOrgUno = "How to train your dragon 3";
            string sinopsisTitulouno = "When Hiccup discovers Toothless isn't the only Night Fury, he must seek " +
            "'The Hidden World', a secret Dragon Utopia before a hired tyrant named Grimmel finds it first.";

            string tituloDos = "El Hombre Invisible";
            string titOrgDos = "The Invisible Man";
            string sinopsisTitulodos = "Se trata sobre un hombre que se hace invisible";

            string tituloTres = "Estafadoras de Wall Street";
            string titOrgtres = "Hustlers";
            string sinopsisTitulotres = "Ellas se la pasan estafando en Wall Street, son...Estafadoras de Wall Street";

            string tituloCuatro = "Mascotas 2";
            string tituloOrgcuatro = "Pets 2";
            string sinopsisTitulocuatro = "Estas mascotas te parcerán adorables";

            Pelicula peliUno = new Pelicula(tituloUno, titOrgUno, directorUno, repartoUno, duracionUno, sinopsisTitulouno);
            Pelicula peliDos = new Pelicula(tituloDos, titOrgDos, directorDos, repartoDos, duracionDos, sinopsisTitulodos);
            Pelicula peliTres = new Pelicula(tituloTres, titOrgtres, directorTres, repartoTres, duracionTres, sinopsisTitulotres); ;
            Pelicula peliCuatro = new Pelicula(tituloCuatro, tituloOrgcuatro, directorCuatro, repartoCuatro, duracionCuatro, sinopsisTitulocuatro);
            #endregion

            #region Funciones
            DateTime fechaUnofuncUno = new DateTime(2020, 11, 24, 15, 30, 00);
            DateTime fechaUnofuncDos = new DateTime(2020, 11, 24, 17, 30, 00);
            DateTime fechaUnofuncTres = new DateTime(2020, 11, 25, 22, 15, 00);
            DateTime fechaDosfuncUno = new DateTime(2020, 11, 26, 14, 30, 00);
            DateTime fechaDosfunDos = new DateTime(2020, 11, 26, 21, 30, 00);
            DateTime fechaTresfuncUno = new DateTime(2020, 11, 29, 11, 30, 00);
            DateTime fechaTresfuncDos = new DateTime(2020, 11, 29, 20, 50, 00);
            DateTime fechaCuatrofuncUno = new DateTime(2020, 11, 29, 16, 15, 00);
            DateTime fechaCuatrofuncDos = new DateTime(2020, 11, 30, 19, 45, 00);
            Funcion funcionUno = new Funcion(peliUno, salaUno, fechaUnofuncUno);
            funcionUno.EstadosAsientos = CompletarEstadoAsientosFunc(salaUno.Asientos);
            Funcion funcionDos = new Funcion(peliUno, salaUno, fechaUnofuncDos);
            funcionDos.EstadosAsientos = CompletarEstadoAsientosFunc(salaUno.Asientos);
            Funcion funcionTres = new Funcion(peliUno, salaUno, fechaUnofuncTres);
            funcionTres.EstadosAsientos = CompletarEstadoAsientosFunc(salaUno.Asientos);
            Funcion funcionCuatro = new Funcion(peliDos, salaDos, fechaDosfuncUno);
            Funcion funcionCinco = new Funcion(peliDos, salaDos, fechaDosfunDos);
            Funcion funcionSeis = new Funcion(peliTres, salaTres, fechaTresfuncUno);
            Funcion funcionSiete = new Funcion(peliCuatro, salaCuatro, fechaCuatrofuncUno);
            Funcion funcionOcho = new Funcion(peliCuatro, salaCuatro, fechaCuatrofuncDos);
            #endregion

            #region Asignaciones a Cine
            unCine.Peliculas.Add(peliUno);
            unCine.Peliculas.Add(peliDos);
            unCine.Peliculas.Add(peliTres);
            unCine.Peliculas.Add(peliCuatro);
            unCine.Funciones.Add(funcionUno);
            unCine.Funciones.Add(funcionDos);
            unCine.Funciones.Add(funcionTres);
            unCine.Funciones.Add(funcionCuatro);
            unCine.Funciones.Add(funcionCinco);
            unCine.Funciones.Add(funcionSeis);
            unCine.Funciones.Add(funcionSiete);
            unCine.Funciones.Add(funcionOcho);
            #endregion

            while (true)
            {
                Iniciar(unCine);
            }

           
        

        }
    }
}
