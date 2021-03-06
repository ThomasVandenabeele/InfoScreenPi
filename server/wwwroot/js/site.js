var baseUrl = document.baseURI;

$( document ).ready( paginationHandler );


$(document).ready(function() {

function listenCheckState(){
    $('.stateCheckItems').unbind('change');
    $('.stateCheckItems').change(function() {
        
        var stateBool =  Boolean($(this).is(":checked"));
        console.log(stateBool);

        var itemId = parseInt((($(this).closest("tr").children("td"))[0]).innerHTML);
        console.log(itemId);

        var postData = { id: itemId, state: stateBool };

        $.ajax({
            type: "POST",
            url: baseUrl + 'Config/Items/ChangeItemState',
            data: postData,
            success: function(response, textStatus, jqXHR){
                console.log(response);
                if(response.success){
                    alertify.success(response.message);
                }
                else{
                    alertify.error(response.message);
                }
            },
            dataType: "json",
            traditional: true
        });

    });
}

window.renewItemsGrid = function(){
    $.get(baseUrl + "Config/Items/Table", function (data) {
        $("#itemsTable").empty();
        $("#itemsTable").html(data);
        $('input.stateCheckItems:checkbox').each(function () {
            $(this).bootstrapToggle();
        });
        listenCheckState();
    });
}
    
listenCheckState();

    var dialogInstance = new BootstrapDialog();
    $("#openArchive").click(function(){

        $.get(baseUrl + "Config/Items/ItemsArchive", function (data) {

                dialogInstance
                    .setTitle('Item Archief')
                    .setMessage($('<div></div>').html(data))
                    .setSize(BootstrapDialog.SIZE_WIDE)
                    .setType(BootstrapDialog.TYPE_PRIMARY)
                    .open();
            });
    
    });

    $( "#nieuwItem" ).click(function() {
        $.get(baseUrl + "Config/Items/CreateItem", function (data) {
                modal
                    .setTitle('Nieuw Item')
                    .setMessage($('<div></div>').html(data))
                    .setSize(BootstrapDialog.SIZE_WIDE)
                    .setType(BootstrapDialog.TYPE_PRIMARY)
                    .open();
            });
    });

    $( "#nieuwVideoItem" ).click(function() {
        $.get(baseUrl + "Config/Items/CreateVideoItem", function (data) {
            modal
                .setTitle('Nieuwe video')
                .setMessage($('<div></div>').html(data))
                .setSize(BootstrapDialog.SIZE_WIDE)
                .setType(BootstrapDialog.TYPE_PRIMARY)
                .open();
        });
    });

    $(document).on('click','.verwijder',function(){
        console.log((($(this).closest("tr").children("td"))[2]));
        var td = (($(this).closest("tr").children("td"))[2]);

        var itemTitle = $(td).children("b").html();

        var itemId = parseInt((($(this).closest("tr").children("td"))[0]).innerHTML);
        console.log(itemId);
        console.log(itemTitle);

        var postData = { id: itemId, state: Boolean(true) };

        alertify.confirm("Bent u zeker dat u '" + itemTitle + "' wilt archiveren?", function (e) {
            if (e) {
                $.ajax({
                    type: "POST",
                    url: baseUrl + 'Config/Items/ArchiveItem',
                    data: postData,
                    success: function(response, textStatus, jqXHR){
                        console.log(response);
                        if(response.success){
                            renewItemsGrid();
                            alertify.success(response.message);
                        }
                        else{
                            alertify.error(response.message);
                        }
                    },
                    dataType: "json",
                    traditional: true
                });
            } else {
                // user clicked "cancel"
            }
        });
    });

    $(document).on('click','.wijzigItem',function(){
        var itemId = parseInt((($(this).closest("tr").children("td"))[0]).innerHTML);

        $.get(baseUrl + "Config/Items/EditCustomItem?id=" + itemId, function (data) {
            modal
                .setTitle('Wijzig Item')
                .setMessage($('<div></div>').html(data))
                .setSize(BootstrapDialog.SIZE_WIDE)
                .setType(BootstrapDialog.TYPE_PRIMARY)
                .open();
        });

        console.log("testWijzig");
    });

    $(document).on('click','.activateItem',function(){
        console.log((($(this).closest("tr").children("td"))[2]));
        var td = (($(this).closest("tr").children("td"))[2]);

        var itemTitle = $(td).children("b").html();

        var itemId = parseInt((($(this).closest("tr").children("td"))[0]).innerHTML);
        console.log(itemId);
        console.log(itemTitle);

        var postData = { id: itemId, state: Boolean(false) };

        alertify.confirm("Bent u zeker dat u '" + itemTitle + "' wilt activeren?", function (e) {
            if (e) {

                dialogInstance.close();
                $.ajax({
                    type: "POST",
                    url: baseUrl + 'Config/Items/ArchiveItem',
                    data: postData,
                    success: function(response, textStatus, jqXHR){
                        console.log(response);
                        if(response.success){
                            renewItemsGrid();
                            alertify.success(response.message);
                        }
                        else{
                            alertify.error(response.message);
                        }
                    },
                    dataType: "json",
                    traditional: true
                });
            } else {
                // user clicked "cancel"
            }
        });
    });
});

//------------------------------------------------------------------------------------------------------------------------------------//




$(function() {

function sendFile(file) {
            
    var formData = new FormData();
    formData.append('file', file);//$('#f_UploadImage')[0].files[0]);
    $.ajax({
        type: 'post',
        url: baseUrl + 'Config/Backgrounds/FileUpload',
        data: formData,
        success: function (response, textStatus, jqXHR) {
            if(response.success){
                refreshGrid();
                $("#placeholderUpload").val("");
                alertify.success(response.message);
            }
            else{
                alertify.error(response.message);
            }
        },
        processData: false,
        contentType: false,
        error: function () {
            alert("Whoops something went wrong!");
        }
    });
}

    var _URL = window.URL || window.webkitURL;

    $("#f_UploadImage").on('change', function () {
    
        var label = $(this).val().replace(/\\/g, '/').replace(/.*\//, '');
        var input = $(this).parents('.input-group').find(':text');

            if( input.length ) {
                input.val(label);
            } else {
                if( label ) alert(log);
            }

        var file, img;
        if ((file = this.files[0])) {
            img = new Image();
            img.onload = function () {
                console.log(file);
                sendFile(file);
            };
            img.onerror = function () {
                alert("Not a valid file:" + file.type);
            };
            img.src = _URL.createObjectURL(file);
        }
    });

function refreshGrid(){

    $.get(baseUrl + "Config/Backgrounds/Grid", function (data) {
        $("#backgroundGrid").empty();
        $("#backgroundGrid").html(data);
        refreshGridMenu();
    });

    
}


function refreshGridMenu(){
    $('#linkAddBackground').click(function() {
        $('input[type=file]#f_UploadImage').trigger('click');
    });
    var context = $('#backgroundGrid')
        .nuContextMenu({

          hideAfterClick: true,

          items: '.demo-item',

          callback: function(key, element) {
            var idb = $(element).attr('id');
            //alert('Clicked ' + key + ' on ' + id);
            var postData = { id: idb };
            $.ajax({
                    type: "POST",
                    url: '/Config/Backgrounds/Delete',
                    data: postData,
                    success: function(response, textStatus, jqXHR){
                        if(response.success){
                            refreshGrid();
                            alertify.success(response.message);
                        }
                        else{
                            alertify.error(response.message);
                        }
                    },
                    dataType: "json",
                    traditional: true
                });
        
         },

          menu: [

            {
              name: 'delete',
              title: 'Delete',
              icon: 'trash',
            },

          ]

        });
      
    
};

refreshGridMenu();

});

//------------------------------------------------------------------------------------------------------------------------------------//
var modal = {};
$(document).ready(function() {

    window.listenCheckStateRss = function(){
        $('.stateCheckRss').unbind('change');
        $('.stateCheckRss').change(function() {
        
            var stateBool =  Boolean($(this).is(":checked"));

            var rssId = parseInt((($(this).closest("tr").children("td"))[0]).innerHTML);

            var postData = { id: rssId, state: stateBool };

            $.ajax({
                type: "POST",
                url: baseUrl + 'Config/RssFeeds/ChangeRssFeedState',
                data: postData,
                success: function(response, textStatus, jqXHR){
                    if(response.success){
                        alertify.success(response.message);
                    }
                    else{
                        alertify.error(response.message);
                    }
                },
                dataType: "json",
                traditional: true
            });
        });
    }

    listenCheckStateRss();

    $( "#RenewRssFeedItems" ).click(function() {
        $.get(baseUrl + "Config/RssFeeds/RenewRssFeeds", function (response) {
            alertify.success(response.message);
                    
        });

    });

    //$("#registerRSS").click(function(){

        modal = new BootstrapDialog();
        $("#registerRSS").click(function(){

            $.get(baseUrl + "Config/RssFeeds/CreateRssFeed", function (data) {

                    modal
                        .setTitle('Nieuw RSS abonnement')
                        .setMessage($('<div></div>').html(data))
                        .setSize(BootstrapDialog.SIZE_WIDE)
                        .setType(BootstrapDialog.TYPE_PRIMARY)
                        .open();

                    
                });
        
        });
        
    $(document).on('click','.verwijderRss',function(){
        var td = (($(this).closest("tr").children("td"))[2]);

        var itemTitle = $(td).children("b").html();

        var rssId = parseInt((($(this).closest("tr").children("td"))[0]).innerHTML);

        var postData = { id: rssId };

        alertify.confirm("Bent u zeker dat u RSS abbonement '" + itemTitle + "' wilt verwijderen?", function (e) {
            if (e) {
                $.ajax({
                    type: "POST",
                    url: baseUrl + 'Config/RssFeeds/DeleteRssFeed',
                    data: postData,
                    success: function(response, textStatus, jqXHR){
                        if(response.success){
                            $.get(baseUrl + "Config/RssFeeds/Table", function (data) {
                                $("#rssFeedsTable").empty();
                                $("#rssFeedsTable").html(data);
                                $('input.stateCheckRss:checkbox').each(function () {
                                    $(this).bootstrapToggle();
                                });
                                listenCheckStateRss();
                            });
                            alertify.success(response.message);
                        }
                        else{
                            alertify.error(response.message);
                        }
                    },
                    dataType: "json",
                    traditional: true
                });
            } else {
                // user clicked "cancel"
            }
        });
    });

    

});


//------------------------------------------------------------------------------------------------------------------------------------//

$(document).ready(function() {

    var autosave_timer;

    function delayedSendTickerLines(){
        if(autosave_timer) {
            clearTimeout(autosave_timer);
        }
        autosave_timer = setTimeout(sendTickerDataLines, 3000); 
    }

    $(".add-more-ticker").click(function(){ 

        var parent = $(this).parents(".control-group");
        var copy = parent.clone(true);
        parent.after(copy);
        parent.next().find("input").val("");

    });

    $(".delete-ticker-line").click(function(){
        $(this).parents(".control-group").remove();
        delayedSendTickerLines();
    });

    $("#parentTickerArea input").keyup(function(){
        delayedSendTickerLines();
    });

    function sendTickerDataLines(){
        var tickerArray = $('#parentTickerArea input').map(function() {
            return this.value;
        }).get();
        var postData = { listkey: tickerArray };

        $.ajax({
            type: "POST",
            url: baseUrl + 'Config/SaveTicker',
            data: postData,
            success: function(response, textStatus, jqXHR){
                if(response.success){
                    $("#parentTickerArea").addClass("has-success");
                    var timerTicker = setTimeout(function(){
                        $("#parentTickerArea").removeClass("has-success");
                    }, 5000);
                    alertify.success(response.message);
                }
                else{
                    $("#parentTickerArea").addClass("has-error");
                    var timerTicker = setTimeout(function(){
                        $("#parentTickerArea").removeClass("has-error");
                    }, 5000);
                    alertify.error(response.message);
                }
            },
            dataType: "json",
            traditional: true
        });
    }

    $(document).on('click','#userDetails',function(){

            $.get(baseUrl + "Config/UserDetails", function (data) {
                modal
                    .setTitle('Gebruiker Details')
                    .setMessage($('<div></div>').html(data))
                    .setSize(BootstrapDialog.SIZE_WIDE)
                    .setType(BootstrapDialog.TYPE_PRIMARY)
                    .open();
            });
        });


         $(document).on('click','#Settings',function(){

            $.get(baseUrl + "Config/GetSettings", function (data) {
                modal
                    .setTitle('Instellingen')
                    .setMessage($('<div></div>').html(data))
                    .setSize(BootstrapDialog.SIZE_WIDE)
                    .setType(BootstrapDialog.TYPE_PRIMARY)
                    .open();
            });
        });



});