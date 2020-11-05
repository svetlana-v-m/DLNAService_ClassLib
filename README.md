# DLNAService_ClassLib

Библиотека позволяет работать с медиаконтентом (фото, видео, аудио), расположенным в локальной сети по протоколу DLNA:
1. Поиск устройств в локальной сети, поддерживающих протокол DLNA.
2. Доступ к медиаконтенту, расположенному на выбранном устройстве из найденных: просмотр перечня файлов и их свойств.

## Classes:
1.    DLNAClient
2.    DLNADevice
3.    DLNAObject
4.    DLNAFolder inherits DLNAObject
5.    DLNAFile inherits DLNAObject

## Library API description
**Start asynchronous searching for DLNADevices:** 

`DLNAClient client=new DLNAClient(); client.StartSearchingForDevices();`

Send message to all devices in local network and get the answer from them.

**Stop searching:** 
Interrupt sending messages/waiting for the answer from the DLNA devices.
`client.StopSearchingForDevices();`

If DLNA device found it is added to ObservableCollection<DLNADevice> DLNADevices in DLNAClient class.
 
Collection can be watched or assigned as a source for some UI element ( client.DLNADevices).
 
**To start working with one of the found DLNA devices** it is necessary to choose it from collection and create new instance of DLNADevice class:

`DLNADevice selectedDevice=new DLNADevice(client.DLNADevices[indexOfSelectedDevice]);`

**To browse in selectedDevice content:** 

`selectedDevice.GetDeviceContent("0")` - for root ets. =>returns List<DLNAObject>
 
**To get filemetadata:** 

`selectedDevice.GetFileInfo("file ID ")` => returns DLNAFile
