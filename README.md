# Magneto.AzureFunctions
Algoritmo detención de mutante<br /><br />
# Arquitectura Empleada para soportar Aplicacion, Visualice: https://1drv.ms/u/s!AtbPjx4g6WKchyNhGx1lBTFv0Z2s<br /><br /><br />
# 1)Flujo de ejecución desde orchestrator:<br /><br />
 1.1)Af-Orchestrator(Orquesta el flujo para validar(Algoritmo) y guardar información) a su vez es el productor de mensajes hacia rabbitMQ<br /><br />
 1.2)Af-Validator(Ejecuta algoritmo de validación de Mutantes)<br />
 esta function contiene variables parametrizables desde setting para ajustes a futuro(Cambio de secuencias o cambio minimo de secuencias,adicion de letras a verificar)<br />
 Contiene a su vez test unitario para ejecutar algoritmo bajo diferentes aspectos (Cantidad de letras, secuencia, secuencia minima) <- todo esto con el fin de tener una solución dinamica <br /><br />
 1.3)RabbitMQ cumple su función de recibir y proveer la mensajeria entre Orchestrator y Laboratory<br /><br />
 1.4)Ms-Laboratory este microservicio hace su trabajo de consumer y administrador hacia MongoDb(Implementa Comandos con MediaTr para la inserciòn hacia la BD)
 <- estara escuchando durante 2 semanas unicamente<br /><br />
 1.5)Af-Stats es el encargado de proveer las metricas<br /><br />
 1.6)MongoDb ClusterMagneto/Magneto es la base de datos que persistira la data<br /><br />
 
# Ejecución completa<br /><br />
Url:https://magnetoazurefunctionsorchestrator20211212175305.azurewebsites.net/api/Mutant<br />
Body:{"dna":[ "ATGCGA", "CAGTGC", "TTATGT", "AGAAGG", "CCCCTA", "TCACTG" ]}<br />
Type:Post<br /><br />

# Ejecución de algoritmo<br /><br />
Url:https://magnetoazurefunctionsvalidator20211211162841.azurewebsites.net/api/validator<br />
Body:{"dna":[ "ATGCGA", "CAGTGC", "TTATGT", "AGAAGG", "CCCCTA", "TCACTG" ]}<br />
Type:Post<br /><br />

# repositorios
Functions: https://github.com/Anseresi5991/Magneto.AzureFunctions<br />
Microservice: https://github.com/Anseresi5991/Magneto.Microservices

# todos los servicios trabajan por consumo, por lo que puede repercutir un poco en el rendimiento en su primera ejecución



