using Microsoft.AspNetCore.Mvc;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;
using System.Threading.Tasks;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
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







    }
}
