using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;
using System.Threading.Tasks;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        //PARA LO NUEVO DE LA V5
        private IMemoryCache memoryCache;

        public EmpleadosController(RepositoryEmpleados repo,IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
        }
       

        public async Task<IActionResult> SessionSalarios(int? salario)
        {

            if (salario != null)
            {
                //QUEREMOS ALMACNAR LA SUMA TOTAL DE SALARIOS
                //QUE TENGAMOS EN SESSION
                int sumaTotal = 0;
                if (HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    //SI YA TENEMOS DATOS ALMACENADOS LOS RECUPERAMOS
                    sumaTotal = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                //SUMAMOS EL NUEVO SALARIO A LA SUMA TOTAL
                sumaTotal += salario.Value;
                //ALMACENAMOS EL ALOR DENTRO DE SESSION
                HttpContext.Session.SetObject("SUMASALARIAL", sumaTotal);
                ViewData["MENSAJE"] = "SALARIO ALMACENADO :" + salario;
            }

            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();

            return View(empleados);
        }
        public IActionResult SumaSalarial()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        //PASAMOS A LA VERSIÓN 2,ALMACENAMOS LOS EMPLEADPOS DE SESSION

        public async Task<IActionResult> SessionEmpleados(int? idempleado)
        {
            if (idempleado != null)
            {
                Empleado empleado =
                await this.repo.FindEmpleadoAsync(idempleado.Value);
                //EN SESSION TENDREMOS ALMACENADOS UN CONJUNTO DE EMPLEADOS 
                List<Empleado> empleadosList;

                //DEBEMOS PREGUNTAR SI YA TENEMOS EMPLEADOS EN SESSION 
                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    //RECUPERAMOS LA LISTA DE SESSION 
                    empleadosList =HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    //CREAMOS UNA NUEVA LISTA PARA ALMACENAR LOS EMPLEADOS 
                    empleadosList = new List<Empleado>();
                }
                //AGREGAMOS EL EMPLEADO AL LIST 
                empleadosList.Add(empleado);
                //ALMACENAMOS LA LISTA EN SESSION 
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewData["MENSAJE"] = "Empleado " + empleado.Apellido
                + " almacenado correctamente";
            }
            List<Empleado> empleados =
            await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }

        //PARA LA VERSIÓN 3 LO MÁS RENTABLE EN CUANTO A RENDIMIENTO SERÍA GUARDAR EL IDDEL EMPLEADO

        public async Task<IActionResult> SessionIdsEmpleados(int? idempleado)
        {
            if (idempleado != null)
            {
                //ALMACENAMOS LO MÍNIMO
                List<int> IdsEmpleados;

                //DEBEMOS PREGUNTAR SI YA TENEMOS EMPLEADOS EN SESSION 
                if (HttpContext.Session.GetObject<List<int>>("IDEMPLEADOS") != null)
                {
                    //RECUPERAMOS LA LISTA DE SESSION 
                    IdsEmpleados = HttpContext.Session.GetObject<List<int>>("IDEMPLEADOS");
                }
                else
                {
                    //CREAMOS UNA NUEVA LISTA PARA ALMACENAR LOS EMPLEADOS 
                    IdsEmpleados = new List<int>();
                }
                //AGREGAMOS EL EMPLEADO AL LIST 
                IdsEmpleados.Add(idempleado.Value);
                //ALMACENAMOS LA LISTA EN SESSION 
                HttpContext.Session.SetObject("IDEMPLEADOS", IdsEmpleados);//REVISAR LO QUE SE GUARDA EN LA COLECCIÓN
                ViewData["MENSAJE"] = "Id: " + idempleado.Value
                + " del empleado  almacenado correctamente";
            }
            
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }




        public async Task<IActionResult> IdsEmpleadosAlmacenados()
        {
            //RECUPERAMOS LOS DATOS DE SESSION
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "NO EXISTEN EMPLEADOS EN SESSION";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosPorColeccion(idsEmpleados);

                return View(empleados);
            }

        }

        /*
         VERSION 4 
No queremos tener empleados repetidos en Session. 
Lo que debemos realizar es mostrar los empleados que NO estén en Session, es decir,  
Si almaceno un empleado en Session, ya no lo mostraré. 
Debemos hacer esta consulta en el caso que tengamos Session. 
         */

        public async Task<IActionResult> SessionEmpleadosV4(int? idempleado)
        {
            if (idempleado != null)
            {
                //ALMACENAMOS LO MÍNIMO
                List<int> IdsEmpleados;

                //DEBEMOS PREGUNTAR SI YA TENEMOS EMPLEADOS EN SESSION 
                if (HttpContext.Session.GetObject<List<int>>("IDEMPLEADOS") != null)
                {
                    //RECUPERAMOS LA LISTA DE SESSION 
                    IdsEmpleados = HttpContext.Session.GetObject<List<int>>("IDEMPLEADOS");
                }
                else
                {
                    //CREAMOS UNA NUEVA LISTA PARA ALMACENAR LOS EMPLEADOS 
                    IdsEmpleados = new List<int>();
                }
                //AGREGAMOS EL EMPLEADO AL LIST 
                IdsEmpleados.Add(idempleado.Value);
                //ALMACENAMOS LA LISTA EN SESSION 
                HttpContext.Session.SetObject("IDEMPLEADOS", IdsEmpleados);//REVISAR LO QUE SE GUARDA EN LA COLECCIÓN
                ViewData["MENSAJE"] = "Id: " + idempleado.Value
                + " del empleado  almacenado correctamente";


                    List<Empleado> empleados = await this.repo.GetEmpleadosSinRepetirAsync(IdsEmpleados);
                    return View(empleados);
                
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSinRepetirAsync(HttpContext.Session.GetObject<List<int>>("IDEMPLEADOS"));
                return View(empleados);
            }


        }

        public async Task<IActionResult> EmpleadosAlmacenadosV4()
        {
            //RECUPERAMOS LOS DATOS DE SESSION
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "NO EXISTEN EMPLEADOS EN SESSION";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosPorColeccion(idsEmpleados);

                return View(empleados);
            }

        }
        //VAMOS A AÑADIRLE EL CACHE DISTIBUIDO,COMO SE VE ESTO NO ES FUNCIONAL ,HABRÍA QUE USAR CACHE 
        //[ResponseCache(Duration =80,Location =ResponseCacheLocation.Client)]
        
        public async Task<IActionResult> SessionEmpleadosV5
            (int? idempleado,int? idfavorito)
        {
            //CODIGO NUEVO PARA EL MEMORYCACHE PERSONALIZADO
            if (idfavorito != null)
            {
                //COMO ESTOY ALMACENANDO EN CACHE, VAMOS A GUARDAR DIRECTAMENTE
                //LOS OBJETOS EN LUGAR DE LOS IDS
                List<Empleado> empleadosFavoritos;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    //NO EXISTE NADA EN CACHE
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    //RECUPERAMOS EL CACHE
                    empleadosFavoritos =
                       this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }
                //BUSCAMOS AL EMPLEADO PARA GUARDARLOS
                Empleado empleadoFavorito = await this.repo.FindEmpleadoAsync(idfavorito.Value);
                empleadosFavoritos.Add(empleadoFavorito);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);
            }



            //MIRAR BIEN EL GETPBJECT DE SESSION QUE TIENE QUE COINCIDIR EL NOMBRE
            if (idempleado != null)
            {
                //ALMACENAMOS LO MINIMO...
                List<int> idsEmpleadosList;
                if (HttpContext.Session.GetObject<List<int>>
                    ("IDSEMPLEADOS") != null)
                {
                    //RECUPERAMOS LA COLECCION
                    idsEmpleadosList =
                        HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                else
                {
                    //CREAMOS LA COLECCION
                    idsEmpleadosList = new List<int>();
                }
                //ALMACENAMOS EL ID DEL EMPLEADO
                idsEmpleadosList.Add(idempleado.Value);
                //ALMACENAMOS EN SESSION LOS DATOS
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleadosList);
                ViewData["MENSAJE"] = "Empleados almacenados: "
                    + idsEmpleadosList.Count;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
            
        }

        //nos creamos un nuevo método para ver los favoritos
        public IActionResult EmpleadosFavoritos()
        {
            /*if (this.memoryCache.Get("FAVORITOS") == null)
            {
                ViewData["MENSAJE"] = "NO HAY EMPLEADOS FAVORITOS";
                return View();
            }
            else
            {
                List<Empleado> favoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                return View(favoritos);
            }*/

            return View();
        }

        //[ResponseCache(Duration = 80, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> EmpleadosAlmacenadosV5(int? ideliminar)
        {
            //RECUPERAMOS LA COLECCION DE SESSION
            List<int> idsEmpleados =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");

            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados en Session";
                return View();
            }
            else
            {
                //PREGUNTAMOS SI HEMOS RECIBIDO ALGUN DATOS PARA ELIMINAR
                if (ideliminar != null)
                {
                    //SI NO TENEMOS EMPLEADOS EN SESSION ,NUESTRA COLECCION EXISTE Y SE QUEDA A  0
                    //ELIMINAMOS SESSION
                        idsEmpleados.Remove(ideliminar.Value);

                    if (idsEmpleados.Count==0)
                    {
                        HttpContext.Session.Remove("IDSEMPLEADOS");
                    }
                    else
                    {
                        //ACTUALIZAMOS SESSION
                        HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    }

                }

                List<Empleado> empleados =
                    await this.repo.GetEmpleadosPorColeccion(idsEmpleados);
                return View(empleados);
            }
        }









    }
}
