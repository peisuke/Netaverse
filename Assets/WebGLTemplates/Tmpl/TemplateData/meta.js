var web3j;
var userAccount;
var userinfoSwapAddress = "0x92c78e22aD635Ee84f30Fd2A9BC6793B76154923";
var atomicSwapAddress = "0xa2b3Bb67F1dCb4d8e4170F20E91E4dB60945a029";

var contractUserinfo;
var contractAtomicSwap;
var contractToken;

$(document).ready(() => {
  if (typeof window.ethereum !== 'undefined') {
    web3js = new Web3(Web3.givenProvider || "ws://localhost:7545");
    contractUserinfo = new web3js.eth.Contract(userinfoABI, userinfoSwapAddress);
    contractAtomicSwap = new web3js.eth.Contract(atomicSwapABI, atomicSwapAddress);
  } else {
    alart("Install Metamask");
  }
});


async function connect() {
  try {
    const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
    userAccount = accounts[0];

    setInterval(function() {
      ethereum.request({ method: 'eth_requestAccounts' })
        .then(accounts => {
          if (accounts[0] !== userAccount) {
            Logout();
          }          
        })
    }, 1000);

  } catch (error) {
    alert(error.message);
    return -1;
  }
  return 0;
}

async function get_username() {
  var userName = ''
  try {
    userName = await contractUserinfo.methods.getName(userAccount).call();
  } catch (error) {
    alert(error.message);
  }
  return userName;
}

async function set_username(name) {
  try {
    console.log(userAccount);
    await contractUserinfo.methods.setName(name).send({from: userAccount});
  } catch (error) {
    alert(error.message);
    return -1;
  }
  return 0;
}

async function remove_username(name) {
  try {
    console.log(userAccount);
    await contractUserinfo.methods.removeName().send({from: userAccount});
  } catch (error) {
    alert(error.message);
    return -1;
  }
  return 0;
}

async function send_transfer(name, token, value) {
  console.log(name, token, value);
  try {
    var targetAddress = await contractUserinfo.methods.getAddress(name).call();
    console.log(targetAddress);
    
    var contract = new web3js.eth.Contract(tokenABI, token);
    console.log(contract);
    const decimals = await contract.methods.decimals().call();
    console.log(value, decimals);
    const bigValue = BigNumber(value).times(BigNumber(10).pow(decimals));

    const numAsHex = "0x" + bigValue.toString(16);
    console.log(targetAddress, numAsHex, userAccount);
    await contract.methods.transfer(targetAddress, numAsHex).send({from: userAccount})
  } catch (error) {
    alert(error.message);
    return -1;
  }
  return 0;
}

async function request_swap(name, openToken, openValue, closeToken, closeValue) {
  console.log(name, openToken, openValue, closeToken, closeValue);
  try {
    var targetAddress = await contractUserinfo.methods.getAddress(name).call();
    console.log(targetAddress);
    
    const swapSeed = Math.random().toString(36) + Math.random().toString(36);
    const swapID = '0x' + keccak256(swapSeed).toString('hex');

    var openContract = new web3js.eth.Contract(tokenABI, openToken);
    const openDecimals = await openContract.methods.decimals().call();
    const openBigValue = BigNumber(openValue).times(BigNumber(10).pow(openDecimals));
    const openBigValueHex = "0x" + openBigValue.toString(16);

    var closeContract = new web3js.eth.Contract(tokenABI, closeToken);
    const closeDecimals = await closeContract.methods.decimals().call();
    const closeBigValue = BigNumber(closeValue).times(BigNumber(10).pow(closeDecimals));
    const closeBigValueHex = "0x" + closeBigValue.toString(16);

    await openContract.methods.approve(atomicSwapAddress, openBigValueHex).send({from: userAccount});
    await contractAtomicSwap.methods.open(swapID, openBigValueHex, openToken, closeBigValueHex, targetAddress, closeToken).send({from: userAccount});
  } catch (error) {
    alert(error.message);
    return -1;
  }
  return 0;
}

async function get_swap_info(data) {
  const openTraderName = await contractUserinfo.methods.getName(data['openTrader']).call();
  const closeTraderName = await contractUserinfo.methods.getName(data['closeTrader']).call();
  
  var openContract = new web3js.eth.Contract(tokenABI, data['openContractAddress']);
  const openDecimals = await openContract.methods.decimals().call();
  const openValue = BigNumber(data['openValue'].toString()).div(BigNumber(10).pow(openDecimals)).toNumber();
  const openSymbol = await openContract.methods.symbol().call()

  var closeContract = new web3js.eth.Contract(tokenABI, data['closeContractAddress']);
  const closeDecimals = await closeContract.methods.decimals().call();
  const closeValue = BigNumber(data['closeValue'].toString()).div(BigNumber(10).pow(closeDecimals)).toNumber();
  const closeSymbol = await closeContract.methods.symbol().call()

  return {
    'openTraderName': openTraderName,
    'closeTraderName': closeTraderName,
    'openValue': openValue,
    'openSymbol': openSymbol,
    'closeValue': closeValue,
    'closeSymbol': closeSymbol
  }
}

async function get_requested_swap_impl(trader) {
  var view = []
  try {
    const ret = await contractAtomicSwap.getPastEvents("Open", { filter: {[trader]: userAccount}, fromBlock: 0 });

    //
    const swapID = ret.map(item => item['returnValues'][0])
    const opened = await Promise.all(swapID.map(async item => await (contractAtomicSwap.methods.isOpened(item).call())))
      .then(bits => swapID.filter(i => bits.shift()))
    const data = await Promise.all(opened.map(async item => await contractAtomicSwap.methods.check(item).call()))
    view = await Promise.all(data.map(async item => await get_swap_info(item)))

    for (var i = 0; i < view.length; i++) {
      view[i]['swapID'] = opened[i]
    }
  } catch (error) {
    alert(error.message);
  }

  return view
}

async function get_requested_swap_to() {
  view = await get_requested_swap_impl("closeTrader")
  return view
}

async function get_requested_swap_from() {
  view = await get_requested_swap_impl("openTrader")
  return view
}

async function close_swap(swapID) {
  try {
    const data = await contractAtomicSwap.methods.check(swapID).call();

    var closeContract = new web3js.eth.Contract(tokenABI, data['closeContractAddress']);
    const closeBigValue = BigNumber(data['closeValue'].toString());
    const closeBigValueHex = "0x" + closeBigValue.toString(16);

    await closeContract.methods.approve(atomicSwapAddress, closeBigValueHex).send({from: userAccount});
    await contractAtomicSwap.methods.close(swapID).send({from: userAccount});
  } catch (error) {
    alert(error.message);
    return -1;
  }
  return 0
}

async function expire_swap(swapID) {
  try {
    await contractAtomicSwap.methods.expire(swapID).send({from: userAccount});
  } catch (error) {
    alert(error.message);
    return -1;
  }
  return 0
}
