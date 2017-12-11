function displayMessage(message) {
    $("#status-message").html(message);
    $("#status-message").animate({top:'60px'}, 1000);
    $("#status-message").animate({top:'60px'}, 2000);
    $("#status-message").animate({top:'-150px'}, 500);
  }