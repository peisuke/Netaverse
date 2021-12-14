async function Connect(parameter) {
  var ret = await connect();
  unityInstance.SendMessage(
    parameter.callbackGameObjectName, 
    parameter.callbackFunctionName,
    ret
  );
}

async function GetUsername(parameter) {
  console.log(parameter)
  var username = await get_username();
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, username)
}

async function SetUsername(parameter) {
  console.log(parameter)
  var ret = await set_username(parameter.nickName)
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, ret)
}

async function RemoveUsername(parameter) {
  console.log(parameter)
  var ret = await remove_username();
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, ret)
}

async function SendTransfer(parameter) {
  console.log(parameter)
  await send_transfer(parameter.name, parameter.tokenAddress, parameter.value);
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName,
                            parameter.name, parameter.tokenAddress, parameter.value)
}

async function RequestSwap(parameter) {
  console.log(parameter)
  await request_swap(parameter.name, 
                     parameter.openTokenAddress, parameter.openValue,
                     parameter.closeTokenAddress, parameter.closeValue);

  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName,
                            parameter.name, 
                            parameter.openTokenAddress, parameter.openValue,
                            parameter.closeTokenAddress, parameter.closeValue)
}

async function GetRequestSwap(parameter) {
  const ret = await get_requested_swap_to();
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, JSON.stringify(ret));
}

async function CloseSwap(parameter) {
  const ret = await close_swap(parameter.swapID);
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, ret);
}

async function GetRequestSwapFrom(parameter) {
  const ret = await get_requested_swap_from();
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, JSON.stringify(ret));
}

async function ExpireSwap(parameter) {
  const ret = await expire_swap(parameter.swapID);
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, ret);
}

async function Logout() {
  unityInstance.SendMessage('SettingCanvas', 'Logout');
}

function recieveMessage(event) {
  var data = JSON.parse(event.detail)
  var methodName = data.methodName
  var parameter = data.parameter
  try {
    parameter = JSON.parse(parameter)
  } catch (e) {
    parameter = null
  }
  eval(`${methodName}(parameter)`)
}

window.addEventListener('unityMessage', recieveMessage, false)
