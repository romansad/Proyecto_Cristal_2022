﻿using AutoMapper;
using MVPSA_V2022.clases;
using MVPSA_V2022.Exceptions;
using MVPSA_V2022.Modelos;

namespace MVPSA_V2022.Services
{
    public class ReclamoService : IReclamoService
    {
        public readonly IMapper _mapper;
        public readonly IUsuarioService _usuarioService;

        public ReclamoService(IMapper mapper, IUsuarioService usuarioService) {
            _mapper = mapper;
            _usuarioService = usuarioService;
        }

        /**
         * RECLAMOS
         */

        public ReclamoCLS guardarReclamo(ReclamoCLS reclamoCLS, int idVecinoAlta)
        {
            try
            {
                Reclamo reclamo = new Reclamo();
                reclamo.CodTipoReclamo = reclamoCLS.codTipoReclamo;
                reclamo.Calle = reclamoCLS.calle;
                reclamo.Altura = reclamoCLS.altura;
                reclamo.EntreCalles = reclamoCLS.entreCalles;
                reclamo.CodEstadoReclamo = 1;
                reclamo.Descripcion = reclamoCLS.descripcion;
                reclamo.Bhabilitado = 1;
                reclamo.IdVecino = idVecinoAlta;
                reclamo.Fecha = DateTime.Now;

                using (M_VPSA_V3Context bd = new M_VPSA_V3Context()) {

                    // Guarda la foto 1 si el vecino la cargo
                    if (reclamoCLS.foto1 != null && reclamoCLS.foto1.Length > 0) {
                        PruebaGraficaReclamo pruebaGraficaReclamo1 = new PruebaGraficaReclamo();
                       // pruebaGraficaReclamo1.Foto = reclamoCLS.foto1;
                        pruebaGraficaReclamo1.IdVecino = idVecinoAlta;
                        pruebaGraficaReclamo1.Bhabilitado = 1;

                        reclamo.PruebaGraficaReclamos.Add(pruebaGraficaReclamo1);
                    }

                    // Guarda la foto 2 si el vecino la cargo
                    if (reclamoCLS.foto2 != null && reclamoCLS.foto2.Length > 0)
                    {
                        PruebaGraficaReclamo pruebaGraficaReclamo2 = new PruebaGraficaReclamo();
                     //   pruebaGraficaReclamo2.Foto = reclamoCLS.foto2;
                        pruebaGraficaReclamo2.IdVecino = idVecinoAlta;
                        pruebaGraficaReclamo2.Bhabilitado = 1;

                        reclamo.PruebaGraficaReclamos.Add(pruebaGraficaReclamo2);
                    }

                    // Guarda el reclamo con las imagenes
                    bd.Reclamos.Add(reclamo);
                    bd.SaveChanges();

                    reclamoCLS.nroReclamo = reclamo.NroReclamo;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("Se produjo un error y no se pudo guardar el Reclamo");
            }
            return reclamoCLS;
        }

        public IEnumerable<ReclamoCLS> listarReclamos()
        {
            using (M_VPSA_V3Context bd = new M_VPSA_V3Context())
            {

                List<ReclamoCLS> listaReclamo = (from reclamo in bd.Reclamos
                                                 join estadoReclamo in bd.EstadoReclamos
                                                 on reclamo.CodEstadoReclamo equals estadoReclamo.CodEstadoReclamo
                                                 join tipoReclamo in bd.TipoReclamos
                                                 on reclamo.CodTipoReclamo equals tipoReclamo.CodTipoReclamo
                                                 where reclamo.Bhabilitado == 1
                                                 select new ReclamoCLS
                                                 {
                                                     nroReclamo = reclamo.NroReclamo,
                                                     Fecha = (DateTime)reclamo.Fecha,
                                                     estadoReclamo = estadoReclamo.Nombre,
                                                     tipoReclamo = tipoReclamo.Nombre,
                                                 }).ToList();
                return listaReclamo;
            }
        }

        /**
         * 
         * Tipos de Reclamo
         * 
         */
        public IEnumerable<TipoReclamoCLS> listarTiposReclamo()
        {
            List<TipoReclamoCLS> listaTiposReclamo;
            using (M_VPSA_V3Context bd = new M_VPSA_V3Context())
            {
                listaTiposReclamo = (from tipoReclamo in bd.TipoReclamos
                                     join usuarioAlta in bd.Usuarios
                                       on tipoReclamo.IdUsuarioAlta equals usuarioAlta.IdUsuario
                                     join usuarioModificacion in bd.Usuarios
                                       on tipoReclamo.IdUsuarioModificacion equals usuarioModificacion.IdUsuario
                                     where tipoReclamo.Bhabilitado == 1
                                    select new TipoReclamoCLS
                                    {
                                        cod_Tipo_Reclamo = tipoReclamo.CodTipoReclamo,
                                        nombre = tipoReclamo.Nombre,
                                        descripcion = tipoReclamo.Descripcion,
                                        tiempo_Max_Tratamiento = tipoReclamo.TiempoMaxTratamiento == null ? 0 :(int) tipoReclamo.TiempoMaxTratamiento,
                                        usuarioAlta = usuarioAlta.NombreUser,
                                        usuarioModificacion = usuarioModificacion.NombreUser
                                    })
                                    .OrderBy(tr => tr.cod_Tipo_Reclamo)
                                    .ToList();
                return listaTiposReclamo;
            }
        }

        public TipoReclamoCLS guardarTipoReclamo(TipoReclamoCLS tipoReclamoDto, int idUsuarioAlta) {

            validarTipoReclamo(tipoReclamoDto);

            TipoReclamo tipoReclamo = new TipoReclamo();
            tipoReclamo.Nombre = tipoReclamoDto.nombre;
            tipoReclamo.Descripcion = tipoReclamoDto.descripcion;
            tipoReclamo.TiempoMaxTratamiento = tipoReclamoDto.tiempo_Max_Tratamiento;
            tipoReclamo.IdUsuarioAlta = idUsuarioAlta;
            tipoReclamo.IdUsuarioModificacion = idUsuarioAlta;
            tipoReclamo.Bhabilitado = 1;

            using (M_VPSA_V3Context bd = new M_VPSA_V3Context()) {
                bd.TipoReclamos.Add(tipoReclamo);
                bd.SaveChanges();

                tipoReclamoDto = (from tipoReclamoQuery in bd.TipoReclamos
                                     join usuarioAlta in bd.Usuarios
                                       on tipoReclamoQuery.IdUsuarioAlta equals usuarioAlta.IdUsuario
                                     join usuarioModificacion in bd.Usuarios
                                       on tipoReclamoQuery.IdUsuarioModificacion equals usuarioModificacion.IdUsuario
                                     where tipoReclamoQuery.CodTipoReclamo == tipoReclamo.CodTipoReclamo
                                  select new TipoReclamoCLS
                                     {
                                         cod_Tipo_Reclamo = tipoReclamoQuery.CodTipoReclamo,
                                         nombre = tipoReclamoQuery.Nombre,
                                         descripcion = tipoReclamoQuery.Descripcion,
                                         tiempo_Max_Tratamiento = tipoReclamoQuery.TiempoMaxTratamiento == null ? 0 : (int)tipoReclamo.TiempoMaxTratamiento,
                                         usuarioAlta = usuarioAlta.NombreUser,
                                         usuarioModificacion = usuarioModificacion.NombreUser
                                     })
                                    .Single();
            }

            return tipoReclamoDto;

        }

        
        public TipoReclamoCLS modificarTipoReclamo(TipoReclamoCLS tipoReclamoDto, int idUsuarioModificacion)
        {
            validarTipoReclamo(tipoReclamoDto);

            TipoReclamo tipoReclamo = new TipoReclamo();
            using (M_VPSA_V3Context bd = new M_VPSA_V3Context())
            {
                tipoReclamo =
                    bd.TipoReclamos.Where(tr => tr.CodTipoReclamo == tipoReclamoDto.cod_Tipo_Reclamo).Single();
                tipoReclamo.Nombre = tipoReclamoDto.nombre;
                tipoReclamo.Descripcion = tipoReclamoDto.descripcion;
                tipoReclamo.TiempoMaxTratamiento = tipoReclamoDto.tiempo_Max_Tratamiento;
                tipoReclamo.FechaModificacion = DateTime.Now;
                tipoReclamo.IdUsuarioModificacion = idUsuarioModificacion;
                bd.SaveChanges();

                tipoReclamoDto = (from tipoReclamoQuery in bd.TipoReclamos
                                  join usuarioAlta in bd.Usuarios
                                    on tipoReclamoQuery.IdUsuarioAlta equals usuarioAlta.IdUsuario
                                  join usuarioModificacion in bd.Usuarios
                                    on tipoReclamoQuery.IdUsuarioModificacion equals usuarioModificacion.IdUsuario
                                  where tipoReclamoQuery.CodTipoReclamo == tipoReclamo.CodTipoReclamo
                                  select new TipoReclamoCLS
                                  {
                                      cod_Tipo_Reclamo = tipoReclamoQuery.CodTipoReclamo,
                                      nombre = tipoReclamoQuery.Nombre,
                                      descripcion = tipoReclamoQuery.Descripcion,
                                      tiempo_Max_Tratamiento = tipoReclamoQuery.TiempoMaxTratamiento == null ? 0 : (int)tipoReclamoQuery.TiempoMaxTratamiento,
                                      fechaAlta = (DateTime)tipoReclamoQuery.FechaAlta,
                                      fechaModificacion = (DateTime)tipoReclamoQuery.FechaModificacion,
                                      usuarioAlta = usuarioAlta.NombreUser,
                                      usuarioModificacion = usuarioModificacion.NombreUser
                                  })
                                    .Single();
            }

            return tipoReclamoDto;
        }

        private void validarTipoReclamo(TipoReclamoCLS tipoReclamoDto)
        {
            if (tipoReclamoDto.nombre == null || tipoReclamoDto.nombre.Length == 0
                || tipoReclamoDto.nombre.Length > 90)
            {
                throw new TipoReclamoNoValidoException("El nombre es requerido y no puede superar los 90 caracteres");
            }

            if (tipoReclamoDto.descripcion == null || tipoReclamoDto.descripcion.Length == 0
                || tipoReclamoDto.descripcion.Length > 250)
            {
                throw new TipoReclamoNoValidoException("La descripción es requerida y no puede superar los 90 caracteres");
            }

            if (tipoReclamoDto.tiempo_Max_Tratamiento < 0)
            {
                throw new TipoReclamoNoValidoException("El tiempo máximo de tratamiento no puede ser inferior a 0");
            }
        }

        public void eliminarTipoReclamo(int codTipoReclamoEliminar)
        {
            using (M_VPSA_V3Context bd = new M_VPSA_V3Context())
            {
                Boolean codTipoReclamoEstaEnUso = 
                    bd.Reclamos.Where(reclamo => reclamo.CodTipoReclamo == codTipoReclamoEliminar).Count() > 0;

                if (codTipoReclamoEstaEnUso) {
                    throw new TipoReclamoEnUsoException("No se puede eliminar el Tipo de Reclamo, hay Reclamos cargados con dicho tipo.");
                }

                TipoReclamo tipoReclamoEliminar = 
                    bd.TipoReclamos.Where(tr => tr.CodTipoReclamo == codTipoReclamoEliminar).Single();
                bd.TipoReclamos.Remove(tipoReclamoEliminar);
                bd.SaveChanges();
            }
        }

        public TipoReclamoCLS getTipoReclamo(int codTipoReclamo) {
            TipoReclamoCLS tipoReclamoResponse;

            using (M_VPSA_V3Context bd = new M_VPSA_V3Context()) {
                tipoReclamoResponse = (from tipoReclamoQuery in bd.TipoReclamos
                                       join usuarioAlta in bd.Usuarios
                                         on tipoReclamoQuery.IdUsuarioAlta equals usuarioAlta.IdUsuario
                                       join usuarioModificacion in bd.Usuarios
                                         on tipoReclamoQuery.IdUsuarioModificacion equals usuarioModificacion.IdUsuario
                                       where tipoReclamoQuery.CodTipoReclamo == codTipoReclamo
                                       select new TipoReclamoCLS
                                       {
                                           cod_Tipo_Reclamo = tipoReclamoQuery.CodTipoReclamo,
                                           nombre = tipoReclamoQuery.Nombre,
                                           descripcion = tipoReclamoQuery.Descripcion,
                                           tiempo_Max_Tratamiento = tipoReclamoQuery.TiempoMaxTratamiento == null ? 0 : (int)tipoReclamoQuery.TiempoMaxTratamiento,
                                           fechaAlta = (DateTime)tipoReclamoQuery.FechaAlta,
                                           fechaModificacion = (DateTime)tipoReclamoQuery.FechaModificacion,
                                           usuarioAlta = usuarioAlta.NombreUser,
                                           usuarioModificacion = usuarioModificacion.NombreUser
                                       }).Single();
            }

            if (tipoReclamoResponse == null) {
                throw new TipoReclamoNotFoundException("No se encontro un Tipo de Reclamo con codigo: " + codTipoReclamo);
            }

            return tipoReclamoResponse;
        }
    }
}
