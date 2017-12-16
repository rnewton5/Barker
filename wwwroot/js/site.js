// used to display a message in a pop-down banner.
function displayMessage(message) {
  $("#status-message").html(message);
  $("#status-message").animate({top:'60px'}, 1000);
  $("#status-message").animate({top:'60px'}, 2000);
  $("#status-message").animate({top:'-150px'}, 500);
}

// attempts to submit a post the server from the text area at the top of the feed.
// displays a message dictating the result
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

// attempts to submit a post the server from the submit post modal
// displays a message dictating the result
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

// submits a request to the server to follow a user.
// displays a message dictating the result
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

// submits a request to the server to like a post.
// displays a message dictating the result
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

$('body').on("click", ".delete-post-button", function(event) {
  event.preventDefault();
  var url = $(this).attr("href");
  var postToDelete = $(this).parent('.post');

  var request = new XMLHttpRequest();
  request.open('GET', url);
  request.onload = function() {
      var jsonData = JSON.parse(request.responseText);
      displayMessage(jsonData.message);
      if(jsonData.message == "Deleted!") {
        $(postToDelete).remove();
      }
  }
  request.send();
})

/* edit post modal functions */
var postId;
// displays the edit post modal
$('body').on("click", ".edit-post-btn", function() {
  var caller = $(this);
  postId = caller.val();
  var text = caller.parent('.post').find('p').text();
  $('#edit-modal-textarea').val(text);
  $('#edit-modal').css({"display": "block"});
})

// submit the edit post form
$('body').on("click", "#edit-modal-submit-btn", function(event) {
  event.preventDefault();
  var $form = $("#edit-modal-form"),
  term = $form.find("textarea[name='Message']").val(),
  url = $form.attr("action");
  url += "?postId=" + postId

  var model = { Message: term };

  var posting = $.post(url, model);
  
  posting.done(function(data) {
    displayMessage(data.message);
    $('#edit-modal').css({"display": "none"});
    $('#edit-modal-textarea').val('');
  })
})

// closes the edit post modal when the x button is pressed
$('body').on("click", ".edit-modal-close", function() {
  $('#edit-modal').css({"display": "none"});
  $('#edit-modal-textarea').val('');
})

// closes the edit post modal when the cancel button is pressed
$('body').on("click", "#edit-modal-cancel-btn", function() {
  $('#edit-modal').css({"display": "none"});
  $('#edit-modal-textarea').val('');
})

// closes the edit post modal when the user click outside of it
$('body').on("click", "#edit-modal", function(event) {
  if ($(event.target).is($('#edit-modal'))){
    $('#edit-modal-textarea').val('');
    $('#edit-modal').css({"display": "none"});
  }
})

