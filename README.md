# censudex-auth-service
## Proyecto que sirve como servicio para una arquitectura de microservicios
## Para levantar el proyecto se deben seguir lo siguientes pasos:

## Requisitos previos:
- .NET 9.0.304
- Visual Studio Code 1.95.3 o superior
- API de usuarios (Referencia en https://github.com/AlbertoLyons/censudex-clients-service.git)

## Instalación
1.- Primero debemos abrir la consola de comandos apretando las siguientes teclas y escribir 'cmd':

- "Windows + R" y escribimos 'cmd'

2.- Ahora debemos crear una carpeta en donde guardar el proyecto, esta carpeta puede estar donde desee el usuario:
```bash
mkdir [NombreDeCarpeta]
```
3.- Accedemoss a la carpeta.
```bash
cd NombreDeCarpeta
```
4.- Se debe clonar el repositorio en el lugar deseado por el usuario con el siguiente comando:
```bash
git clone https://github.com/AlbertoLyons/censudex-auth-service.git
```
5.- Accedemos a la carpeta creada por el repositorio:
```bash
cd censudex-auth-service
```
6.- Ahora debemos restaurar las dependencias del proyecto con el siguiente comando:
```bash
dotnet restore
```
7.- Con las dependencias restauradas, abrimos el editor:
```bash
code .
```
8.- Establecer las credenciales del archivo .env
```bash
notepad .env
```
9.- Finalmente ya en el editor ejecutamos el siguiente comando para ejecutar el proyecto:
```bash
dotnet run
```

## Estructura del repositorio
- Funciona con una API de tipo REST
- El proyecto se conecta a una API GRPC
- Se ofrece una colección en la carpeta de Auth de Postman dandole [click a este enlace](https://pm5555-1180.postman.co/workspace/censudex~d35666a5-243c-48a5-a764-be2248d7173f/collection/68f97e8bd2eca7ecf70c0480?action=share&creator=34959437)
- Se ofrece un .env de con datos de ejemplo
- Se utiliza el Framework .NET de C#
- El proyecto como módulo se debe de conectar a una main API
- Utiliza endpoints para realizar el CRUD del servicio de la API de usuarios
- Se utiliza la ruta "http://localhost:5001" para realizar las peticiones HTTP