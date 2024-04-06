using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Proyecto_de_topicos_Final.Models;

namespace Proyecto_de_topicos_Final.ViewModels
{
    public enum Ventanas { AgregarTemporada, EditarTemporada, EliminarTemporada, ListaTemporadas,ListaEpisodios,
        Inicio, AgregarEpisodio, EditarEpisodio,EliminarEpisodio}
    public class SerieViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Temporadas> Series { get; set; } = new();
        public string TextoFiltro { get; set; }
        public IEnumerable<Temporadas> filtroTemporadas => Series.Where(x=>x.Titulo==TextoFiltro).Select(x=>x);
        public IEnumerable<Temporadas> ordenTemporadas => Series.OrderBy(x=>x.NumeroTemporada).Select(x=>x);
        public IEnumerable<Episodios> EpisodiosOrdenados => Temporada.Episodios.OrderBy(x => x.NumeroEpisodio).Select(x => x);
        public ICommand AgregarTemporadaCommand { get; set; }
        public ICommand AgregarEpisodioCommand { get; set; }
        public ICommand EliminarTemporadaCommand { get; set; }
        public ICommand EliminarEpisodioCommand { get; set; }
        public ICommand EditarTemporadaCommand { get; set; }
        public ICommand EditarEpisodioCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }
        public Temporadas? Temporada { get; set; }
        public Episodios? Episodio { get; set; }
        public string Error { get; set; } = "";
        public Ventanas Ventana { get; set; } = Ventanas.Inicio;

        public SerieViewModel()
        {
            EliminarEpisodioCommand = new RelayCommand(EliminarEpisodio);
            AgregarEpisodioCommand = new RelayCommand(AgregarEpisodio);
            EditarEpisodioCommand = new RelayCommand(EditarEpisodio);
            AgregarTemporadaCommand = new RelayCommand(AgregarTemporada);
            EditarTemporadaCommand = new RelayCommand(EditarTemporada);
            EliminarTemporadaCommand = new RelayCommand(EliminarTemporada);
            CambiarVistaCommand = new RelayCommand<Ventanas>(CambiarVista);
            Deserializar(); //cargar
        }

        private void EliminarTemporada()
        {
            if (Temporada != null)
            {
                Series.Remove(Temporada);
                Serializar();
                Ventana = Ventanas.ListaTemporadas;
                Actualizar(nameof(Ventana));
                Actualizar(nameof(ordenTemporadas));
            }
           
        }
        private void EliminarEpisodio()
        {
            if (Episodio != null && Temporada != null)
            {
                Temporada.Episodios.Remove(Episodio);
                Serializar();
                Ventana = Ventanas.ListaEpisodios;
                Actualizar(nameof(Ventana));
                Actualizar(nameof(EpisodiosOrdenados));

            }
        }



        int posAnterior;
        private void CambiarVista(Ventanas vistas)
        {

            if (vistas == Ventanas.AgregarTemporada)
            {
                Temporada = new();
            }
            if (vistas == Ventanas.AgregarEpisodio && Temporada!=null)
            {
                Episodio = new();
            }

            if (vistas == Ventanas.EditarTemporada && Temporada != null)
            {
                var clon = new Temporadas
                {
                    Descripcion = Temporada.Descripcion,
                    NumeroTemporada = Temporada.NumeroTemporada,
                    Titulo = Temporada.Titulo,
                    URLimagen = Temporada.URLimagen
                };

                posAnterior = Series.IndexOf(Temporada);

                Temporada = clon;
            }
            if (vistas == Ventanas.EditarEpisodio && Episodio != null && Temporada!=null)
            {
                var clon = new Episodios
                {
                    Descripcion = Episodio.Descripcion,
                    NumeroEpisodio = Episodio.NumeroEpisodio,
                    Titulo = Episodio.Titulo,
                    URLimagen = Episodio.URLimagen
                };

                posAnterior = Temporada.Episodios.IndexOf(Episodio);

                Episodio = clon;
            }

            Error = "";
            Actualizar(nameof(Error));

            Ventana = vistas;
            Actualizar(nameof(Ventana));

            Actualizar(nameof(Temporada));
            Actualizar(nameof(Episodio));
            Actualizar(nameof(ordenTemporadas));

        }

        private void EditarTemporada()
        {
            if (Temporada != null)
            {
                Error = "";

                //Validar
                if (Series.Any(x => x.NumeroTemporada == Temporada.NumeroTemporada) && Temporada.NumeroTemporada<=0)
                {
                    Error = "Escriba un numero de temporada valido.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Titulo))
                {
                    Error = "Escriba el titulo de la temporada.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Descripcion))
                {
                    Error = "Escriba la descripcion de la temporada";
                }
                if (string.IsNullOrWhiteSpace(Temporada.URLimagen) || !Temporada.URLimagen.StartsWith("http")
                   || !Temporada.URLimagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }

                Actualizar(nameof(Error));


                if (Error == "")//Si no hay error lo guardo
                {
                   var episodiosGuardados = Series[posAnterior].Episodios;
                    Series[posAnterior] = Temporada; //Reemplazo en la posicion original del libro a editar
                    Temporada.Episodios = episodiosGuardados;
                    Serializar();

                    Ventana = Ventanas.ListaTemporadas;
                    Actualizar(nameof(Ventana));
                    Actualizar(nameof(ordenTemporadas));
                }

            }
        }
        private void EditarEpisodio()
        {
            if (Episodio!=null && Temporada != null)
            {
                Error = "";

                //Validar
                if (string.IsNullOrWhiteSpace(Episodio.Titulo))
                {
                    Error = "Escriba el titulo del Episodio.";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Descripcion))
                {
                    Error = "Escriba la descripcion del episodio";
                }
                if (string.IsNullOrWhiteSpace(Episodio.URLimagen) || !Episodio.URLimagen.StartsWith("http")
                   || !Episodio.URLimagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }

                Actualizar(nameof(Error));


                if (Error == "")//Si no hay error lo guardo
                {
                    Temporada.Episodios[posAnterior] = Episodio; //Reemplazo en la posicion original del libro a editar
                    Serializar();

                    Ventana = Ventanas.ListaEpisodios;
                    Actualizar(nameof(Ventana));
                    Actualizar(nameof(EpisodiosOrdenados));
                }

            }
        }

        private void AgregarTemporada()
        {
          
            if (Temporada != null)
            {
                Error = "";
                if (Series.Any(x => x.NumeroTemporada == Temporada.NumeroTemporada) || Temporada.NumeroTemporada<=0)
                {
                    Error = "Escriba un numero de temporada valido.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Titulo))
                {
                    Error = "Escriba el titulo de la temporada.";
                }
                if (string.IsNullOrWhiteSpace(Temporada.Descripcion))
                {
                    Error = "Escriba la descripcion de la temporada";
                }
                if (string.IsNullOrWhiteSpace(Temporada.URLimagen) || !Temporada.URLimagen.StartsWith("http")
                     || !Temporada.URLimagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }

                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Series.Add(Temporada);              
                Serializar();
                Ventana = Ventanas.ListaTemporadas;
                Actualizar(nameof(Ventana));
                Actualizar(nameof(ordenTemporadas));

            }
        }
        private void AgregarEpisodio()
        {
            if (Episodio!=null && Temporada != null)
            {
                Error = "";
                if (Temporada.Episodios.Any(x => x.NumeroEpisodio == Episodio.NumeroEpisodio) || Episodio.NumeroEpisodio<=0)
                {
                    Error = "Escriba un numero de episodio valido.";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Titulo))
                {
                    Error = "Escriba el titulo del episodio.";
                }
                if (string.IsNullOrWhiteSpace(Episodio.Descripcion))
                {
                    Error = "Escriba la descripcion del episodio";
                }
                if (string.IsNullOrWhiteSpace(Episodio.URLimagen) || !Episodio.URLimagen.StartsWith("http")
                     || !Episodio.URLimagen.EndsWith(".jpg"))
                {
                    Error = "Escriba la dirección de una imagen en JPG.";
                }              

                if (!string.IsNullOrWhiteSpace(Error))
                {
                    Actualizar(nameof(Error));
                    return;
                }
                Temporada.Episodios.Add(Episodio);
                Serializar();
                Ventana = Ventanas.ListaEpisodios;
                Actualizar(nameof(Ventana));
                Actualizar(nameof(EpisodiosOrdenados));
            }
        }

        void Serializar()
        {
            File.WriteAllText("Series.json", JsonSerializer.Serialize(Series));
        }
        void Deserializar()
        {
            try
            {
                var datos = JsonSerializer.Deserialize<ObservableCollection<Temporadas>>(File.ReadAllText("Series.json"));             
                if (datos != null)
                {
                    foreach (var Item in datos)
                    {
                        Series.Add(Item);
                    }
                }
            }
            catch { }
        }
        private void Actualizar(string? name)
        {
            PropertyChanged?.Invoke(this, new(name));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
