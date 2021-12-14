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

# Consultar Estadistica<br /><br />
Url:https://magnetoazurefunctionsstat20211213135037.azurewebsites.net/api/stats<br />
Type:Get<br /><br />

# Ejecución de algoritmo<br /><br />
Url:https://magnetoazurefunctionsvalidator20211211162841.azurewebsites.net/api/validator<br />
Body:{"dna":[ "ATGCGA", "CAGTGC", "TTATGT", "AGAAGG", "CCCCTA", "TCACTG" ]}<br />
Type:Post<br /><br />
# =========================================<br />
# Explicación Algoritmo<br /><br />
Entrada: array de Strings que representan cada fila de una tabla<br />
de (NxN) con la secuencia del ADN. Las letras de los Strings solo pueden ser: (A,T,C,G)(Esto quedò parametrizable por appsetting), las<br />
cuales representa cada base nitrogenada del ADN. Sabrás si un humano es mutante, si encuentras más de una secuencia(Quedò parametrizable por appsetting)<br />
de cuatro(Quedò parametrizable por appsetting) letras<br />
iguales, de forma oblicua, horizontal o vertical.<br /><br />

Ejemplo de Array Mutante {"ATGCGA","CAGTGC","TTATGT","AGAAGG","CCCCTA","TCACTG"} y para el ejercicio llamaremos data<br /><br />
A)Se crea array de char dnac que se le asigna el valor parametrizado "Letters" del appsettings que para el ejercicios es Igual a ['A','C','T','G']<br /><br />
B)Se crea Variable secuenceMin y se le asignara el valor parametrizado SecuenceMin del appsettings que para el ejercicio es Igual a 2<br /><br />
C)se crea Variable secuenceLetters y se le asignara el valor parametrizado SecuenceLetters del appsettings que para el ejercicio es igual a 4<br /><br />
D)Se transforma data en un array de char con nombre chars, quedando de la siguiente manera: <br />
# ['A','T','G','C','G','A','C','A','G','T','G','C','T','T','A','T','G','T','A','G','A','A','G','G','C','C','C','C','T','A','T','C','A','C','T','G']<br /><br />
E)se crea variable countChar y se le asigna el valor de cantidad de caracteres de la primera cadena de strings de data (countChar = dta[0].count()) = 6<br /><br />
F) se crea variable countLetter y se le asigna el valor de cantidad de letras de dnac (countLetter = dnac.count()) = 4<br /><br />
G) se crea variable secuence y se inicializa en 0 para llevar el conteo de las secuencias que se llevan durante la ejecución del algoritmo<br /><br />
H) se inicia ciclo for con los siguientes parametros de ejecucion for (int r = 0; (r < countLetter && secuence < secuenceMin); r++)<br /><br />
I) se obtienen por cada iteraciòn del ciclo cada letra de dnac para ser evaluada<br /><br />
J) se obtiene la cantidad de veces que se encuentra repetida la letra evaluada<br /><br />
K) se inicia ciclo anidado con los siguientes parametros de ejecucion (int i = 0; (i < count && secuence < secuenceMin); i++) que obtiene la primera posiciòn de la letra dentro del array de char chars <br /)<br />
L) se obtiene la primera posicion de la letra<br /><br />
M) se evalua si a su derecha, hay posiciones suficientes para alcanzar la secuencia minima de letras (availablePositions>=secuenceLetters)<br /><br />
M.1) en caso de estar disponibles las posiciones minimas, se evalua si en la siguiente posición hacia la derecha existe la misma letra<br /><br />
M.2) si no existe la misma letra sale de la validación, en caso de que si exista la misma letra evalua la siguiente posiciòn y asi sucesivamente hasta alcanzar la secuenceLetter<br /><br />
N) se evalua si a abajo hay posiciones suficientes para alcanzar la secuencia minima de letras (availablePositions>=secuenceLetters)<br /><br />
N.1)en caso de estar disponibles las posiciones minimas, se evalua si en la siguiente posición hacia abajo existe la misma letra<br /><br />
N.2)si no existe la misma letra sale de la validación, en caso de que si exista la misma letra evalua la siguiente posiciòn y asi sucesivamente hasta alcanzar la secuenceLetter<br /><br />
Ñ)se evalua si en la oblicua desplegada a la derecha hay posiciones suficientes para alcanzar la secuencia minima de letras (availablePositions>=secuenceLetters)<br /><br />
Ñ.1)en caso de estar disponibles las posiciones minimas, se evalua si en la siguiente posición hacia en la oblicua desplegada a la derecha existe la misma letra<br /><br />
Ñ.2)si no existe la misma letra sale de la validación, en caso de que si exista la misma letra evalua la siguiente posiciòn y asi sucesivamente hasta alcanzar la secuenceLetter<br /><br />
O)se evalua si en la oblicua desplegada a la izquierda hay posiciones suficientes para alcanzar la secuencia minima de letras (availablePositions>=secuenceLetters)<br /><br />
O.1)en caso de estar disponibles las posiciones minimas, se evalua si en la siguiente posición hacia en la oblicua desplegada a la izquierda existe la misma letra<br /><br />
O.2)si no existe la misma letra sale de la validación, en caso de que si exista la misma letra evalua la siguiente posiciòn y asi sucesivamente hasta alcanzar la secuenceLetter<br /><br />
P)Si alguna de las validaciones de las diferentes direcciones encuentra la secuencia la variable secuence aumentara en 1<br /><br />
Q)esto se repetira por cada letra de dnac<br /><br />
R)si secuence alcanza el valor 2 no se ejecuta mas ninguna iteración de los ciclos<br /><br />
S)si secuence == 2 el algoritmo retorna true(EsMutante) y por ende el servicio que lo ejecuta el status 200 Ok<br /><br />
T)si al evaluar todas las letras secuence es < 2 el algoritmo retorna false(NoEsMutante) y por ende el servicio que lo ejecuta el status 406 Forbidden<br /><br />

# =========================================<br /><br />
Luego de la validación del ADN se procesa la información para ser guardada en MongoDB en dos colecciones, una para mutantes y otra para humanos, esto quiere decir que segun<br /> 
la logica los ADN con respuesta 406 Forbidden se guardaran en la de humanos y respuestas 200 Ok en la de mutantes(esto para poder obtener las estadisticas con el servicios stats)

# Repositorios
Functions: https://github.com/Anseresi5991/Magneto.AzureFunctions<br />
Microservice: https://github.com/Anseresi5991/Magneto.Microservices

# todos los servicios trabajan por consumo, por lo que puede repercutir un poco en el rendimiento en su primera ejecución



