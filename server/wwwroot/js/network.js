var defDate = $.fullCalendar.moment('2018-12-24');
//var testInput = "{\"screenOn\":[{\"day\":2,\"hour\":9,\"minutes\":0},{\"day\":3,\"hour\":8,\"minutes\":45},{\"day\":4,\"hour\":9,\"minutes\":45},{\"day\":5,\"hour\":7,\"minutes\":30},{\"day\":6,\"hour\":10,\"minutes\":45}],\"screenOff\":[{\"day\":2,\"hour\":11,\"minutes\":15},{\"day\":3,\"hour\":10,\"minutes\":30},{\"day\":4,\"hour\":11,\"minutes\":0},{\"day\":5,\"hour\":9,\"minutes\":30},{\"day\":6,\"hour\":12,\"minutes\":15}]}";
var defEventTitle = "SCHERM AAN";
var deviceId;

function pad(num, size) {
    var s = num+"";
    while (s.length < size) s = "0" + s;
    return s;
}

$(function() {

    $("#SaveOperatingTable").click(function(e){
        sendTimeSlots($("#fullCalendar").fullCalendar('clientEvents'));
    });

    // page is now ready, initialize the calendar...

    $('#fullCalendar').fullCalendar({
        aspectRatio: 2,
        locale: 'nl-be',
        handleWindowResize: true,
        defaultView: 'agenda',
        header: false,
        defaultDate: defDate,
        themeSystem: 'bootstrap3',
        columnFormat: 'dddd',
        duration: { days: 7 },
        allDaySlot: false,
        navLinks: false, // can click day/week names to navigate views
        selectable: true,
        selectHelper: true,
        slotDuration: '00:15:00',
        slotLabelInterval: '00:30',
        slotLabelFormat: 'HH:mm',
        select: function(start, end) {
            var eventData;
            if (defEventTitle) {
                eventData = {
                    title: defEventTitle,
                    start: start,
                    end: end//,
                    //id: 1  //DEZE MOET NOG UNIEK WORDEN
                };
                $('#fullCalendar').fullCalendar('renderEvent', eventData, true); // stick? = true
            }
            $('#fullCalendar').fullCalendar('unselect');
        },
        editable: true,
        eventLimit: true, 
        eventRender: function(event, element) {
            element.bind('dblclick', function() {
                //alert('double click!'); // BEVESTIGING NOODZAKELIJK?
                $('#fullCalendar').fullCalendar('removeEvents', event._id);
                //sendTimeSlots($("#fullCalendar").fullCalendar('clientEvents'));
            });
        },
        eventOverlap: false,
        viewRender: function(view, element){
            placeTimeSlots(tabelString);
        }

    });


    function placeTimeSlots(screenEventListJSON){
        try {
            var screenEventList = JSON.parse(screenEventListJSON); // this is how you parse a string into JSON 
            for (i = 0; i < screenEventList.screenOn.length; i++){
                var start = screenEventList.screenOn[i];
                var end = screenEventList.screenOff[i];

                var evStart = moment(defDate).add(start.day-1, 'days');
                var evEnd = moment(defDate).add(end.day-1, 'days');

                var momStart = evStart.format() + "T" + pad(start.hour, 2) + ":" + pad(start.minutes, 2);
                var momEnd = evEnd.format() + "T" + pad(end.hour, 2) + ":" + pad(end.minutes, 2);

                $('#fullCalendar').fullCalendar( 'renderEvent', {
                    title: defEventTitle,
                    start: momStart,
                    end: momEnd
                } );
            }

        } catch (ex) {
            console.error(ex);
        }
    }


    function sendTimeSlots(eventList){

        var screenOnList = [];
        var screenOffList = [];
        eventList.forEach(function(event) {

            var start = {
                day: event.start.isoWeekday(),
                hour: event.start.hours(),
                minutes: event.start.minutes()
            };
            screenOnList.push(start);

            var end = {
                day: event.end.isoWeekday(),
                hour: event.end.hours(),
                minutes: event.end.minutes()
            };
            screenOffList.push(end);

        });
        var screenEventList = {
            screenOn: screenOnList.sort(function(a, b) { return a.day - b.day}),
            screenOff: screenOffList.sort(function(a, b) { return a.day - b.day})
        };
        var postData = {
            deviceId: deviceId,
            operatingString: JSON.stringify(screenEventList)
        };
        
        $.ajax({
            type: "POST",
            url: '/Config/SaveOperatingTimes',
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
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            traditional: true
        });

    }

});