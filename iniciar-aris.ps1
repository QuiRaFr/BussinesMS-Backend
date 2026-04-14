# Script para iniciar A.R.I.S. con bienvenida
# Uso: .\iniciar-aris.ps1

param(
    [string]$Mensaje = ""
)

# Cambiar al directorio del proyecto
Set-Location "C:\Users\Frank_QR\Desktop\Mishel\ProyectoMS\BackEnd"

# Mostrar bienvenida
Clear-Host
Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  A.R.I.S. - Automated Resource & Inventory System" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Bienvenido a BussinesMS!" -ForegroundColor Green
Write-Host ""
Write-Host "  ==========================================================" -ForegroundColor Yellow
Write-Host "  COMO USARME:" -ForegroundColor Yellow
Write-Host "  ==========================================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "  Para CREAR modulos:" -ForegroundColor White
Write-Host "     'Crear modulo de Productos'" -ForegroundColor Gray
Write-Host "     'Agregar entidad Cliente'" -ForegroundColor Gray
Write-Host "     'Necesito un servicio de Inventario'" -ForegroundColor Gray
Write-Host ""
Write-Host "  Para CORREGIR/MODIFICAR:" -ForegroundColor White
Write-Host "     'Hay un error en el servicio de...'" -ForegroundColor Gray
Write-Host "     'Actualizar el skill de...'" -ForegroundColor Gray
Write-Host "     'Corregir el codigo de...'" -ForegroundColor Gray
Write-Host ""
Write-Host "  Para INFORMACION:" -ForegroundColor White
Write-Host "     'Que modulos existen?'" -ForegroundColor Gray
Write-Host "     'Como esta estructurado el proyecto?'" -ForegroundColor Gray
Write-Host "     'Explicar como funciona...'" -ForegroundColor Gray
Write-Host ""
Write-Host "  ==========================================================" -ForegroundColor Yellow
Write-Host "  DETECCION AUTOMATICA:" -ForegroundColor Yellow
Write-Host "  ==========================================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "  - Palabras como 'crear', 'agregar', 'modulo' = Subagente" -ForegroundColor DarkGray
Write-Host "  - Palabras como 'corregir', 'actualizar' = Agente Principal" -ForegroundColor DarkGray
Write-Host "  - Preguntas = Respuesta directa" -ForegroundColor DarkGray
Write-Host ""
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Escribe tu solicitud y presiona Enter..." -ForegroundColor Green
Write-Host ""

# Leer el mensaje inicial del usuario
if ($Mensaje) {
    Write-Host "  Tu solicitud: $Mensaje" -ForegroundColor White
    Write-Host ""
}

# Iniciar opencode
opencode