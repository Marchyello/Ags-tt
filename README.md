# Ags-tt

The project is driven by three Azure Functions with distinct functionality:
  
## TimedRequestSender
A timer-triggered Azure Function that sends out a request to a fixed URL every minute, stores some of its meta-information in an Azure Storage table and the payload inside an Azure Storage blob.

## ResponseMetaLister
An HTTP triggered Azure Function (`GET` to: `api/meta-range`) that fetches a range of meta-information stored by **TimedRequestSender**. The fetched range can be limited by providing bounds as query parameters `from` and `to`.  
Example request URL: `http://your.base/api/range?from=2020-08-26T08:12&to=2020-08-26T08:15`.  
  
## ResponsePayloadGetter
An HTTP triggered Azure Function (`GET` to: `api/payload/{key}`) that fetches a single payload stored by **TimedRequestSender** and identified by its `{key}`. The `{key}` of the payload can be acquired from the results of **ResponseMetaLister** where it is exposed as `blobKey`.  
  
#### NOTE: Don't forget to add your own `local.settings.json`.