var NativeAdapter = require('edge').func(__dirname + '\\NodeWrapper\\bin\\Debug\\NodeWrapper.dll');
var adapter = {};

NativeAdapter(null, function(error, native) {
  if (error) console.error(error);

  Object.keys(native).forEach(function (cmd) {
    adapter[cmd] = function() {
      var args = Array.prototype.slice.call(arguments);
      native[cmd](args, function(error, result) {
        if (error) console.error(error);
        if (result) console.log(result);
      })
    }
  });
});

process.stdin.resume();
