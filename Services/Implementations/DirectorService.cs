using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolManager.Services.Interfaces;
using SchoolManager.ViewModels;

namespace SchoolManager.Services.Implementations
{
    public class DirectorService : IDirectorService
    {
        private readonly IUserService _userService;
        private readonly IStudentReportService _studentReportService;
        private readonly ISubjectService _subjectService;
        private readonly ITrimesterService _trimesterService;

        public DirectorService(IUserService userService, IStudentReportService studentReportService, ISubjectService subjectService, ITrimesterService trimesterService)
        {
            _userService = userService;
            _studentReportService = studentReportService;
            _subjectService = subjectService;
            _trimesterService = trimesterService;
        }

        public async Task<DirectorViewModel> GetDashboardViewModelAsync(string trimestre = null)
        {
            var model = new DirectorViewModel();
            var trimestres = await _trimesterService.GetAllAsync();
            model.TrimestresDisponibles = trimestres;
            model.TrimestreSeleccionado = string.IsNullOrEmpty(trimestre) ? "" : trimestre;

            // Obtener datos de estudiantes, aprobados, reprobados, etc.
            var estudiantes = await _userService.GetAllAsync();
            var soloEstudiantes = estudiantes.Where(e => e.Role.ToLower() == "estudiante" || e.Role.ToLower() == "student" || e.Role.ToLower() == "alumno").ToList();
            model.TotalEstudiantes = soloEstudiantes.Count;
            int totalAprobados = 0;
            int totalReprobados = 0;

            var reportesPorEstudiante = new Dictionary<Guid, SchoolManager.Dtos.StudentReportDto>();
            foreach (var estudiante in soloEstudiantes)
            {
                try
                {
                    var reporte = await _studentReportService.GetReportByStudentIdAsync(estudiante.Id);
                    // Si no se filtra por trimestre, incluir todos los reportes
                    if (reporte != null && (string.IsNullOrEmpty(model.TrimestreSeleccionado) || reporte.Trimester == model.TrimestreSeleccionado))
                        reportesPorEstudiante[estudiante.Id] = reporte;
                }
                catch { }
            }

            foreach (var estudiante in soloEstudiantes)
            {
                if (reportesPorEstudiante.TryGetValue(estudiante.Id, out var reporte) && reporte.Grades != null && reporte.Grades.Count > 0)
                {
                    var promedio = reporte.Grades.Average(g => (double)g.Value);
                    if (promedio >= 3.0)
                        totalAprobados++;
                    else if (promedio >= 1.0 && promedio < 3.0)
                        totalReprobados++;
                }
                else
                {
                    totalReprobados++;
                }
            }

            double porcentajeAprobados = model.TotalEstudiantes > 0 ? (totalAprobados * 100.0 / model.TotalEstudiantes) : 0;
            double porcentajeReprobados = model.TotalEstudiantes > 0 ? (totalReprobados * 100.0 / model.TotalEstudiantes) : 0;

            model.TotalAprobados = totalAprobados;
            model.TotalReprobados = totalReprobados;
            model.PorcentajeAprobados = porcentajeAprobados;
            model.PorcentajeReprobados = porcentajeReprobados;

            var materias = await _subjectService.GetAllAsync();
            var materiasDesempeno = new List<MateriaDesempenoViewModel>();
            foreach (var materia in materias)
            {
                int estudiantesMateria = 0;
                int aprobadosMateria = 0;
                int reprobadosMateria = 0;
                double sumaPromedios = 0;
                int totalPromedios = 0;

                foreach (var estudiante in soloEstudiantes)
                {
                    if (reportesPorEstudiante.TryGetValue(estudiante.Id, out var reporte) && reporte.Grades != null)
                    {
                        var notasMateria = reporte.Grades.Where(g => g.Subject == materia.Name).ToList();
                        if (notasMateria.Count > 0)
                        {
                            estudiantesMateria++;
                            var promedioMateria = notasMateria.Average(g => (double)g.Value);
                            sumaPromedios += promedioMateria;
                            totalPromedios++;
                            if (promedioMateria >= 3.0)
                                aprobadosMateria++;
                            else if (promedioMateria >= 1.0 && promedioMateria < 3.0)
                                reprobadosMateria++;
                        }
                    }
                }
                double promedioFinal = totalPromedios > 0 ? sumaPromedios / totalPromedios : 0;
                materiasDesempeno.Add(new MateriaDesempenoViewModel
                {
                    Nombre = materia.Name,
                    Estudiantes = estudiantesMateria,
                    Promedio = Math.Round(promedioFinal, 1),
                    Aprobados = aprobadosMateria,
                    Reprobados = reprobadosMateria,
                    ColorBarra = promedioFinal >= 4.0 ? "#27ae60" : "#f1c40f"
                });
            }

            var profesores = await _userService.GetAllWithAssignmentsByRoleAsync("teacher");
            var profesoresDesempeno = new List<ProfesorDesempenoViewModel>();
            foreach (var prof in profesores)
            {
                var asignaciones = prof.TeacherAssignments;
                if (asignaciones == null || asignaciones.Count == 0)
                    continue;

                var materiasProfesor = asignaciones.Select(a => a.SubjectAssignment?.Subject?.Name).Distinct().Where(n => !string.IsNullOrEmpty(n)).ToList();
                int profTotalEstudiantes = 0;
                double profSumaPromedios = 0;
                int profTotalPromedios = 0;
                int profTotalAprobados = 0;
                int profTotalReprobados = 0;
                DateTime? ultimaActividad = null;

                foreach (var materiaNombre in materiasProfesor)
                {
                    foreach (var reporte in reportesPorEstudiante.Values)
                    {
                        var notas = reporte.Grades.Where(g => g.Subject == materiaNombre && g.Teacher == prof.Name).ToList();
                        if (notas.Count > 0)
                        {
                            profTotalEstudiantes++;
                            var promedio = notas.Average(g => (double)g.Value);
                            profSumaPromedios += promedio;
                            profTotalPromedios++;
                            if (promedio >= 3.0)
                                profTotalAprobados++;
                            else if (promedio >= 1.0 && promedio < 3.0)
                                profTotalReprobados++;
                            var fechaUltima = notas.Max(g => g.CreatedAt);
                            if (!ultimaActividad.HasValue || fechaUltima > ultimaActividad)
                                ultimaActividad = fechaUltima;
                        }
                    }
                }
                double promedioGeneral = profTotalPromedios > 0 ? profSumaPromedios / profTotalPromedios : 0;
                double porcentajeAprobadosProf = profTotalEstudiantes > 0 ? (profTotalAprobados * 100.0 / profTotalEstudiantes) : 0;
                double porcentajeReprobadosProf = profTotalEstudiantes > 0 ? (profTotalReprobados * 100.0 / profTotalEstudiantes) : 0;
                string estado = "Crítico";
                if (porcentajeAprobadosProf >= 80) estado = "Excelente";
                else if (porcentajeAprobadosProf >= 60) estado = "Regular";

                profesoresDesempeno.Add(new ProfesorDesempenoViewModel
                {
                    Nombre = prof.Name,
                    Materia = string.Join(", ", materiasProfesor),
                    Desempeno = Math.Round(promedioGeneral, 1),
                    Estudiantes = profTotalEstudiantes,
                    Promedio = Math.Round(promedioGeneral, 1),
                    Aprobados = profTotalAprobados,
                    PorcentajeAprobados = porcentajeAprobadosProf,
                    Reprobados = profTotalReprobados,
                    PorcentajeReprobados = porcentajeReprobadosProf,
                    UltimaActividad = ultimaActividad ?? DateTime.MinValue,
                    Estado = estado
                });
            }

            double promedioGeneralActual = 0;
            int totalPromediosGlobal = 0;
            foreach (var reporte in reportesPorEstudiante.Values)
            {
                if (reporte.Grades != null && reporte.Grades.Count > 0)
                {
                    promedioGeneralActual += reporte.Grades.Average(g => (double)g.Value);
                    totalPromediosGlobal++;
                }
            }
            promedioGeneralActual = totalPromediosGlobal > 0 ? promedioGeneralActual / totalPromediosGlobal : 0;

            var materiasAprobacion = new List<MateriaAprobacionViewModel>();
            foreach (var mat in materiasDesempeno)
            {
                var profesor = profesoresDesempeno.FirstOrDefault(p => p.Materia.Contains(mat.Nombre));
                double porcentajeAprobacion = mat.Estudiantes > 0 ? (mat.Aprobados * 100.0 / mat.Estudiantes) : 0;
                materiasAprobacion.Add(new MateriaAprobacionViewModel
                {
                    Nombre = mat.Nombre,
                    Profesor = profesor?.Nombre ?? "-",
                    TotalEstudiantes = mat.Estudiantes,
                    Aprobados = mat.Aprobados,
                    Reprobados = mat.Reprobados,
                    PorcentajeAprobacion = porcentajeAprobacion
                });
            }

            var recomendaciones = new List<string>();
            var materiaBajo = materiasDesempeno.OrderBy(m => m.Promedio).FirstOrDefault();
            var materiaAlto = materiasDesempeno.OrderByDescending(m => m.Promedio).FirstOrDefault();
            if (materiaBajo != null)
                recomendaciones.Add($"Implementar plan de refuerzo para la materia de {materiaBajo.Nombre}");
            if (materiaAlto != null)
                recomendaciones.Add($"Extender las estrategias exitosas de {materiaAlto.Nombre} a otras materias");
            var profDestacado = profesoresDesempeno.OrderByDescending(p => p.Desempeno).FirstOrDefault();
            if (profDestacado != null)
                recomendaciones.Add($"Reconocer el desempeño destacado del profesor {profDestacado.Nombre}");

            var alertas = new List<AlertaNotificacionViewModel>();
            foreach (var mat in materiasDesempeno)
            {
                if (mat.Promedio < 3.0)
                {
                    alertas.Add(new AlertaNotificacionViewModel
                    {
                        Tipo = "Bajo",
                        Titulo = $"Bajo rendimiento en {mat.Nombre}",
                        Mensaje = $"El promedio de calificaciones en {mat.Nombre} está por debajo del objetivo."
                    });
                }
                if (mat.Estudiantes == 0)
                {
                    alertas.Add(new AlertaNotificacionViewModel
                    {
                        Tipo = "Critico",
                        Titulo = $"Materia sin estudiantes: {mat.Nombre}",
                        Mensaje = $"No hay estudiantes inscritos en la materia {mat.Nombre}."
                    });
                }
                if (mat.Estudiantes > 0 && mat.Reprobados * 100.0 / mat.Estudiantes > 40)
                {
                    alertas.Add(new AlertaNotificacionViewModel
                    {
                        Tipo = "Bajo",
                        Titulo = $"Alto porcentaje de reprobados en {mat.Nombre}",
                        Mensaje = $"Más del 40% de los estudiantes reprobaron {mat.Nombre}."
                    });
                }
            }
            foreach (var prof in profesoresDesempeno)
            {
                if (prof.Estado == "Excelente")
                {
                    alertas.Add(new AlertaNotificacionViewModel
                    {
                        Tipo = "Excelente",
                        Titulo = $"Excelente desempeño de {prof.Nombre}",
                        Mensaje = $"El profesor {prof.Nombre} mantiene un desempeño destacado en sus materias."
                    });
                }
                if (prof.Estudiantes == 0)
                {
                    alertas.Add(new AlertaNotificacionViewModel
                    {
                        Tipo = "Critico",
                        Titulo = $"Profesor sin asignaciones: {prof.Nombre}",
                        Mensaje = $"El profesor {prof.Nombre} no tiene estudiantes asignados actualmente."
                    });
                }
            }
            alertas.Add(new AlertaNotificacionViewModel
            {
                Tipo = "Reporte",
                Titulo = "Reporte mensual disponible",
                Mensaje = "El reporte de desempeño docente de este mes está listo para su revisión."
            });

            model.MateriasDesempeno = materiasDesempeno;
            model.Profesores = profesoresDesempeno;
            model.TasaAprobacionGeneral = porcentajeAprobados;
            model.PromedioGeneralActual = Math.Round(promedioGeneralActual, 1);
            model.PromedioGeneralAnterior = 0;
            model.IncrementoPromedio = 0;
            model.MateriasAprobacion = materiasAprobacion;
            model.Alertas = alertas;
            model.Recomendaciones = recomendaciones;

            return model;
        }
    }
} 