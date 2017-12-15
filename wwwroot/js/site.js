function displayMessage(message) {
  $("#status-message").html(message);
  $("#status-message").animate({top:'60px'}, 1000);
  $("#status-message").animate({top:'60px'}, 2000);
  $("#status-message").animate({top:'-150px'}, 500);
}

$("#submit-post-button").click(function(event) {
  event.preventDefault();
  var $form = $("#submit-post-form"),
    term = $form.find("textarea[name='Message']").val(),
    url = $form.attr("action");

  var model = { Message: term };

  var posting = $.post( url, model);

  posting.done(function(data) {
    displayMessage(data.message);
    $('#barkModal').modal( 'hide' );
    $('.bark-textarea').val('');
  })
})

$("#modal-submit-btn").click(function(event) {
  event.preventDefault();
  var $form = $("#modal-submit-post-form"),
  term = $form.find("textarea[name='Message']").val(),
  url = $form.attr("action");

  var model = { Message: term };

  var posting = $.post( url, model);

  posting.done(function(data) {
    displayMessage(data.message);
    $('#barkModal').modal( 'hide' );
    $('.modal-textarea').val('');
  })
})

$('body').on("click", ".follow-button", function(event){
  event.preventDefault();
  var url = $(this).attr("href");

  var request = new XMLHttpRequest();
  request.open('GET', url);
  request.onload = function() {
      var jsonData = JSON.parse(request.responseText);
      displayMessage(jsonData.message);
      if(jsonData.message == "Followed!") {
        $('a[href="' + url + '"]').html("- Unfollow");
      } else {
        $('a[href="' + url + '"]').html("+ Follow");
      }
  }
  request.send();
})

$('body').on("click", ".like-post-button", function(event){
  event.preventDefault();
  var caller = $(this);
  var url = caller.attr("href");

  var request = new XMLHttpRequest();
  request.open('GET', url);
  request.onload = function() {
      var jsonData = JSON.parse(request.responseText);
      displayMessage(jsonData.message);
      if(jsonData.message == "Liked!"){
        caller.css({"color": "red"});
      } else {
        caller.css({"color": "gray"});
      }
  }
  request.send();
})

