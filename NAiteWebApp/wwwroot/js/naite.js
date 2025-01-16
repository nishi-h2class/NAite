var naite = naite || {};

$(function () {

    naite.loading = function (visible) {
        if (visible) {
            $('#pageloading-wrap').show();
        }
        else {
            $('#pageloading-wrap').hide();
        }
    };

    naite.post = function (url, data, dataType = 'json') {
        this.removeMsgParam();
        this.hiddenMessage();
        return this.ajax("POST", url, JSON.stringify(data), dataType);
    };
    naite.get = function (url, data) {
        return this.ajax("GET", url, data);
    };
    naite.put = function (url, data) {
        this.removeMsgParam();
        this.hiddenMessage();
        return this.ajax("PUT", url, JSON.stringify(data));
    };
    naite.delete = function (url, data) {
        this.removeMsgParam();
        this.hiddenMessage();
        return this.ajax("DELETE", url, JSON.stringify(data));
    };
    naite.FileUpload = function (url, formData) {
        this.removeMsgParam();
        this.hiddenMessage();
        return this.Upload("POST", url, formData);
    };
    naite.FileDownload = function (url, data) {
        return this.Download("GET", url, data);
    };
    naite.ajax = function (method, url, data, dataType) {
        return $.ajax({
            url: url,
            type: method,
            headers: {
                "Content-Type": "application/json",
                "Authorization": function () {
                    //処理
                    var accesstoken = $.cookie('accessToken');
                    if (accesstoken === undefined) {
                        return "";
                    }
                    else {
                        return "Bearer " + accesstoken;
                    }
                }()
            },
            dataType: dataType,
            data: data
        });
    };

    naite.Upload = function (method, url, formData) {
        return $.ajax({
            url: url,
            type: method,
            headers: {
                //"Content-Type": "application/json",
                "Authorization": function () {
                    //処理
                    var accesstoken = $.cookie('accessToken');
                    if (accesstoken === undefined) {
                        return "";
                    }
                    else {
                        return "Bearer " + accesstoken;
                    }
                }()
            },
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
        });
    };

    naite.Download = function (method, url, data) {
        return $.ajax({
            url: url,
            type: method,
            headers: {
                //"Content-Type": "application/json",
                "Authorization": function () {
                    //処理
                    var accesstoken = $.cookie('accessToken');
                    if (accesstoken === undefined) {
                        return "";
                    }
                    else {
                        return "Bearer " + accesstoken;
                    }
                }()
            },
            data: data,
            beforeSend: function (xhr) {
                xhr.overrideMimeType("text/plain; charset=shift_jis");
            },
        });
    };

    naite.redirect = function (url) {
        window.location.href = url;
    };

    naite.logout = function () {
        // 二重クリック防止
        naite.loading(true);

        naite.get(naite.apiUrls.logout)
            .done(function (data) {
                if (data.result.AccessToken) {
                    $.cookie('accessToken', data.result.AccessToken, { path: "/", domain: $("#Domain").val()/*, secure: true */ });
                    naite.redirect(naite.urls.logout_substitute);
                }
                else {
                    $.removeCookie('accessToken');
                    naite.redirect(naite.urls.logout_login);
                }
            })
            .fail(function (error) {
                console.log(error);
                // naite.handleErrorで、naite.loading(false);
                // をしている
                naite.handleError(error);
            });
    };

    naite.loginHandleError = function (error) {
        console.log('error');
        console.log(error);

        // クリック解除
        naite.loading(false);

        // 認証失敗
        if (error.status === 408) {
            naite.redirect(naite.urls.login_timeout);
        }
        if (error.status === 401) {
            naite.redirect(naite.urls.login_unauthorized);
        }
        else if (error.status === 400) {
            var result = error.responseText;
            if (result === undefined || result === null) {
                $('#result').empty();
                $('#result').append($('<ul></ul>').addClass('list-unstyled').addClass('custom-error-msg').append($('<li></li>').text(error.responseJSON.message)));
                $('#result').parent().addClass('has-error').addClass('has-danger');
            }
            else {
                for (var key in result) {
                    var errorKey = key.slice(0, 1).toLowerCase() + key.slice(1);
                    naite.vuemodel.errors.add({ field: errorKey, msg: result[key] });
                }
            }
        }
    };

    naite.handleError = function (error) {
        console.log('error');
        console.log(error);

        // クリック解除
        naite.loading(false);

        // 認証失敗
        if (error.status === 408) {
            naite.redirect(naite.urls.login_timeout);
        }
        if (error.status === 401) {
            naite.redirect(naite.urls.login_unauthorized);
        }
        if (error.status === 404) {
            naite.redirect(naite.urls.notFound);
        }
        else if (error.status === 400) {
            if (error.responseJSON === undefined || error.responseJSON === null) {
                return error.responseText;
            }
            else {
                for (var key in error.responseJSON.errors) {
                    var errorKey = key.slice(0, 1).toLowerCase() + key.slice(1);
                    console.log(errorKey);
                    naite.vuemodel.errors.add({ field: errorKey, msg: error.responseJSON.errors[key][0] });
                }
            }
        }
    };

    naite.getParam = function () {
        // URLのパラメータを取得
        var urlParam = location.search.substring(1);
        // パラメータを格納する用の配列を用意
        var params = {};
        // URLにパラメータが存在する場合
        if (urlParam) {
            // 「&」が含まれている場合は「&」で分割
            var param = urlParam.split('&');
            // 用意した配列にパラメータを格納
            for (i = 0; i < param.length; i++) {
                var paramItem = param[i].split('=');
                params[paramItem[0]] = decodeURIComponent(paramItem[1]);
            }
        }
        return params;
    };

    naite.checkParam = function () {
        var params = naite.getParam();
        for (var key in params) {
            if (key === "created") {
                naite.showCreated();
            } else if (key === "updated") {
                naite.showUpdated();
            }
            else if (key === "deleted") {
                naite.showDeleted();
            }
            else if (key === "reserved") {
                naite.showReserved();
            }
            else if (key === "deleteReserved") {
                naite.showDeleteReserved();
            }
            else if (key === "msg") {
                naite.showMessage(params[key]);
            }
        }
    };

    naite.getPathId = function () {
        var path = $(location).attr('pathname');
        var array = path.split('/');
        var _id = array[array.length - 1];
        array = _id.split('?');
        var id = array[0];
        return id;
    };

    naite.showMessage = function (msg) {
        $("#msg").html('<div class="alert alert-info alert-dismissible col-12"><span>' + msg + '</span><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button></div>');
    };
    naite.errorMessage = function (msg) {
        $("#msg").html('<div class="alert alert-danger alert-dismissible col-12"><span>' + msg + '</span><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></div>');
    };
    naite.appendMessage = function (msg) {
        $("#msg").append('<div class="alert alert-info alert-dismissible col-12"><span>' + msg + '</span><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></div>');
    };

    naite.hiddenMessage = function () {
        $("#msg").html(null);
    };

    naite.showCreated = function (msg) {
        var m = this.messages.created;
        if (msg) {
            m = msg;
        }
        naite.showMessage(m);
    };

    naite.showUpdated = function (msg) {
        var m = this.messages.updated;
        if (msg) {
            m = msg;
        }
        naite.showMessage(m);
    };

    naite.showDeleted = function (msg) {
        var m = this.messages.deleted;
        if (msg) {
            m = msg;
        }
        naite.showMessage(m);
    };

    naite.showReserved = function (msg) {
        var m = this.messages.reserved;
        if (msg) {
            m = msg;
        }
        naite.showMessage(m);
    };

    naite.showDeleteReserved = function (msg) {
        var m = this.messages.deleteReserved;
        if (msg) {
            m = msg;
        }
        naite.showMessage(m);
    };

    naite.messages = {
        created: "登録が完了しました。",
        updated: "登録が完了しました。",
        deleted: "削除が完了しました。",
        reserved: "予約が完了しました。",
        deleteReserved: "削除予約が完了しました。"
    };

    naite.messagesParam = {
        created: "created=1",
        updated: "updated=1",
        deleted: "deleted=1",
        reserved: "reserved=1",
        deleteReserved: "deleteReserved=1"
    };

    naite.removeMsgParam = function () {
        var replaceQueryString = "";
        var paramArray = this.getParam();
        $.each(paramArray, function (key, val) {
            console.log(key + ": " + val);
            // 該当するクエリストリングは無視
            if (key === "created" || key === "deleted" || key === "updated" || key === "reserved" || key === "deleteReserved" || key === "msg") return true;
            // 新たにクエリストリングを作成
            if (replaceQueryString !== "") {
                replaceQueryString += "&";
            } else {
                replaceQueryString += "?";
            }
            replaceQueryString += key + "=" + val;
        });
        // URLに新しいクエリストリングを付与
        window.history.replaceState(null, null, $(location).attr('pathname') + replaceQueryString);
    };

    naite.setOptionName = function (select, data, selected) {
        for (var i = 0; i < data.result.length; i++) {
            var item = data.result[i];
            var option = $("<option>")
                .val(item.Name)
                .text(item.Name);
            if (item.Name === selected) {
                option.attr("selected", "selected");
            }
            $(select).append(option);
        }
    };

    naite.getQueryStringFromSelectValText = function (select, csv) {
        var keys = csv.split(',');
        var result = "";
        for (var i = 0; i < keys.length; i++) {
            if (result !== "") result += ",";
            result += keys[i];
            result += "|" + select.find('option[value=' + keys[i] + ']').text();
        }
        return result;
    };


    naite.dateTimeOffsetDayToString = function (datetimeoffset, separater = "/") {
        if (!datetimeoffset || datetimeoffset == 'null') {
            return '';
        }
        try {
            var dt = new Date(datetimeoffset);
            var m = ("00" + (dt.getMonth() + 1)).slice(-2);
            var d = ("00" + dt.getDate()).slice(-2);
            var result = m + separater + d;
            return result;
        }
        catch (err) {
            return '';
        }
    }

    naite.dateTimeOffsetToString = function (datetimeoffset, separater = "/") {
        if (!datetimeoffset || datetimeoffset == 'null') {
            return '';
        }
        try {
            var dt = new Date(datetimeoffset);
            var y = dt.getFullYear();
            var m = ("00" + (dt.getMonth() + 1)).slice(-2);
            var d = ("00" + dt.getDate()).slice(-2);
            var result = y + separater + m + separater + d;
            return result;
        }
        catch (err) {
            return '';
        }
    }

    naite.dateTimeOffsetYearMonthToString = function (datetimeoffset, separater = "/") {
        if (!datetimeoffset || datetimeoffset == 'null') {
            return '';
        }
        try {
            var dt = new Date(datetimeoffset);
            var y = dt.getFullYear();
            var m = ("00" + (dt.getMonth() + 1)).slice(-2);
            var d = ("00" + dt.getDate()).slice(-2);
            var result = y + separater + m;
            return result;
        }
        catch (err) {
            return '';
        }
    }

    naite.dateTimeOffsetFullToString = function (datetimeoffset, separater = "/") {
        if (!datetimeoffset || datetimeoffset == 'null') {
            return '';
        }
        try {
            var dt = new Date(datetimeoffset);
            var y = dt.getFullYear();
            var m = ("00" + (dt.getMonth() + 1)).slice(-2);
            var d = ("00" + dt.getDate()).slice(-2);
            var h = ("00" + dt.getHours()).slice(-2);
            var mm = ("00" + dt.getMinutes()).slice(-2);
            var s = ("00" + dt.getSeconds()).slice(-2);
            var result = y + separater + m + separater + d + " " + h + ":" + mm;
            //var result = y + separater + m + separater + d + " " + h + ":" + mm + ":" + s;
            return result;
        }
        catch (err) {
            return '';
        }
    }

    naite.dateTimeOffsetSecFullToString = function (datetimeoffset, separater = "/") {
        if (!datetimeoffset || datetimeoffset == 'null') {
            return '';
        }
        try {
            var dt = new Date(datetimeoffset);
            var y = dt.getFullYear();
            var m = ("00" + (dt.getMonth() + 1)).slice(-2);
            var d = ("00" + dt.getDate()).slice(-2);
            var h = ("00" + dt.getHours()).slice(-2);
            var mm = ("00" + dt.getMinutes()).slice(-2);
            var s = ("00" + dt.getSeconds()).slice(-2);
            var result = y + separater + m + separater + d + " " + h + ":" + mm + ":" + s;
            return result;
        }
        catch (err) {
            return '';
        }
    }

    naite.dateTimeOffsetTimeToString = function (datetimeoffset) {
        if (!datetimeoffset || datetimeoffset == 'null') {
            return '';
        }
        try {
            var dt = new Date(datetimeoffset);
            var h = ("00" + dt.getHours()).slice(-2);
            var mm = ("00" + dt.getMinutes()).slice(-2);
            var result = h + ":" + mm;
            return result;
        }
        catch (err) {
            return '';
        }
    }

    naite.getNowYMD = function (separater = "/") {
        var dt = new Date();
        var y = dt.getFullYear();
        var m = ("00" + (dt.getMonth() + 1)).slice(-2);
        var d = ("00" + dt.getDate()).slice(-2);
        var result = y + separater + m + separater + d;
        return result;
    }

    naite.getDefaultDateTime = function (separater = "/") {
        var dt = new Date();
        var y = dt.getFullYear();
        var m = ("00" + (dt.getMonth() + 1)).slice(-2);
        var d = ("00" + dt.getDate()).slice(-2);
        var result = y + separater + m + separater + d + " 00:00";
        return result;
    }

    naite.getDefaultYearMonth = function (separater = "/") {
        var dt = new Date();
        var y = dt.getFullYear();
        var m = ("00" + (dt.getMonth() + 1)).slice(-2);
        var result = y + separater + m;
        return result;
    }

    naite.getCommaNum = function (num) {
        if (num == null || num == '' || num == undefined) {
            return num;
        }
        var s = String(num).split('.');
        var ret = String(s[0]).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, '$1,');
        if (s.length > 1) {
            ret += '.' + s[1];
        }
        return ret;
    }

    naite.getSortValue = function (_this) {
        $(_this).removeClass('active');
        var arrow = $(_this).attr('class');
        $('th[data-role="sort"]').removeClass().removeAttr('data-sortvalue').addClass('sorting');
        $(_this).removeClass();

        var value = $(_this).attr('data-value');
        if (arrow === 'sorting_asc') {
            $(_this).addClass('sorting_desc');
            value = "-" + value;
        } else {
            $(_this).addClass('sorting_asc');
        }

        $(_this).addClass('active');
        $(_this).attr('data-sortvalue', value);
    };

    naite.setSortValueByParam = function (val) {
        var sortValue = val.replace('-', '');
        var sort = $('th[data-value="' + sortValue + '"]');
        if (!sort[0]) {
            sort = $('th.active');
            val = $(sort).attr('data-sortvalue');
        }

        $(sort).removeClass('active');
        var arrow = $(sort).attr('class');
        $('th[data-role="sort"]').removeClass().removeAttr('data-sortvalue').addClass('sorting');
        $(sort).removeClass();

        var value = $(sort).attr('data-value');

        if (val.slice(0, 1) === '-') {
            $(sort).addClass('sorting_desc');
            value = "-" + value;
        }
        else {
            $(sort).addClass('sorting_asc');
        }

        $(sort).addClass('active');
        $(sort).attr('data-sortvalue', value);
        $('#sort').val(value);
    };

    // getIsExistPrePage
    naite.getIsExistPrePage = function (totalPages, pageNo) {
        var result = false;
        if (totalPages > 1 && pageNo > 1) {
            result = true;
        }
        // resultを返す
        return result;
    };

    // getIsExistNextPage
    naite.getIsExistNextPage = function (totalPages, pageNo) {
        var result = false;
        if (totalPages > 1 && pageNo < totalPages) {
            result = true;
        }
        // resultを返す
        return result;
    };

    // getBtnList
    naite.getBtnList = function (totalPages, pageNo, visiblePages) {

        console.log('totalPages:' + totalPages);
        console.log('pageNo:' + pageNo);
        console.log('visiblePages:' + visiblePages);

        var array = [];

        visiblePages = visiblePages - 1;
        var a = visiblePages % 2;
        var bf_visiblePage = Math.floor(visiblePages / 2);
        var af_visiblePage = bf_visiblePage + a;
        console.log('bf_visiblePage:' + bf_visiblePage);
        console.log('af_visiblePage:' + af_visiblePage);

        var startBtn = 0;
        if ((pageNo - bf_visiblePage) <= 0) {
            startBtn = 1;
        }
        else {
            startBtn = pageNo - bf_visiblePage;
        }

        var endBtn = 0;
        if ((pageNo + af_visiblePage) > totalPages) {
            endBtn = totalPages;
            if (endBtn === 0) endBtn = 1;
        }
        else {
            endBtn = pageNo + af_visiblePage;
        }

        var startCount = startBtn - (pageNo - bf_visiblePage);
        var endCount = (pageNo + af_visiblePage) - endBtn;

        startBtn = 0;
        if ((pageNo - (bf_visiblePage + endCount)) <= 0) {
            startBtn = 1;
        }
        else {
            startBtn = pageNo - (bf_visiblePage + endCount);
        }

        endBtn = 0;
        if ((pageNo + (af_visiblePage + startCount)) > totalPages) {
            endBtn = totalPages;
            if (endBtn === 0) endBtn = 1;
        }
        else {
            endBtn = pageNo + (af_visiblePage + startCount);
        }


        for (var i = startBtn; i <= endBtn; i++) {
            array.push(i);
        }

        console.log('BtnList:' + array);
        return array;
    };

    // ページングの作成
    naite.createPagination = function (totalPages, pageNo, visiblePages) {
        $('.pagination').empty();

        //先頭へボタン
        //前へボタン
        if (this.getIsExistPrePage(totalPages, pageNo)) {
            $('.pagination').append('<li class="page-item hidden-xs-down"><a class="page-link" href="#" aria-label="First"><i class="fa fa-angle-double-left"></i></a></li>');
            $('.pagination').append('<li class="page-item"><a class="page-link" href="#" aria-label="Previous"><i class="fa fa-angle-left"></i></a></li>');
        }
        var btnArray = naite.getBtnList(totalPages, pageNo, visiblePages);
        $.each(btnArray, function (i, value) {
            console.log('btnIndex:' + i);
            console.log('btnValue:' + value);
            //<li class="page-item"><a class="page-link" href="#">2</a></li>
            $('.pagination').append('<li class="page-item" data-no="' + value + '"><a class="page-link" href="#">' + value + '</a></li>');
            if (value === pageNo) {
                $('.pagination li[data-no= "' + value + '"]').addClass('active');
            }
        });

        //次へボタン
        if (this.getIsExistNextPage(totalPages, pageNo)) {
            $('.pagination').append('<li class="page-item"><a class="page-link" href="#" aria-label="Next"><i class="fa fa-angle-right"></i></a></li>');
            $('.pagination').append('<li class="page-item hidden-xs-down" data-totalpages="' + totalPages + '"><a class="page-link" href="#" aria-label="Last"><i class="fa fa-angle-double-right"></i></a></li>');
        }
    };

    naite.initPagination = function (getPage) {
        $(document).on('click', '.pagination li a', function (e) {
            console.log('test');
            var result = naite.checkPageActive(this);
            if (result === true) return;
            var ariaLabel = $(this).attr("aria-label");
            var pg = $('.pagination li.active').text();
            console.log($(location).attr('pathname'));
            if (ariaLabel === "First") {
                pg = 1;
                console.log("pg=" + pg);
                getPage(pg);
            }
            else if (ariaLabel === "Previous") {
                pg--;
                console.log("pg=" + pg);
                getPage(pg);
            }
            else if (ariaLabel === "Next") {
                pg++;
                console.log("pg=" + pg);
                getPage(pg);
            }
            else if (ariaLabel === "Last") {
                pg = $(this).parent('li').data('totalpages');
                console.log("pg=" + pg);
                getPage(pg);
            }
            else {
                pg = $(this).parent('li').data('no');
                naite.setActivePage(pg);
                console.log("pg=" + pg);
                getPage(pg);
            }
            var path = location.pathname.toLowerCase();
            if (path == '' || path == '/' || path == '/home') {
                clearInterval(naite.vuemodel.$data.intervalId);
                naite.vuemodel.$data.intervalId = setInterval(() => {
                    getPage(pg);
                }, naite.vuemodel.$data.interval);
            }
        });
    }

    //
    naite.checkPageActive = function (_this) {
        var parentClass = $(_this).parent().attr("class");
        console.log(parentClass);
        var array = parentClass.split(' ');
        console.log(array);
        var num = $.inArray('active', array);
        console.log(num);
        var result = (num > 0 ? true : false);
        return result;
    };

    // ページングのactiveをセットする(TODO バグってる？)
    naite.setPageActive = function (_this) {
        var parentClass = $(_this).parent().attr("class");
        var _pageNo = $('.pagination li.active').data('no');
        var pageNo = 1;
        if (parentClass === "page-no") {
            $('.pagination li').removeClass('active');
            $(_this).parent('li').addClass('active');
        }
        else if (parentClass === "page-prev") {
            pageNo = parseInt(_pageNo) - parseInt(1);
            $('.pagination li').removeClass('active');
            $('.pagination li[data-no="' + pageNo + '"]').addClass('active');
        }
        else {
            pageNo = parseInt(_pageNo) + parseInt(1);
            $('.pagination li').removeClass('active');
            $('.pagination li[data-no="' + pageNo + '"]').addClass('active');
        }
    };

    naite.setActivePage = function (pg) {
        $('.pagination li').removeClass('active');
        $(".pagination li[data-no=" + pg + "]").addClass("active");
    }

    //Dateオブジェクトを文字列に変換
    naite.dateFormatToString = function (date, format) {

        var year_str = ("000" + date.getFullYear()).substr(-4, 4);
        //月だけ+1すること
        var month_str = ("0" + (1 + date.getMonth())).substr(-2, 2);
        var day_str = ("0" + date.getDate()).substr(-2, 2);
        var hour_str = ("0" + date.getHours()).substr(-2, 2);
        var minute_str = ("0" + date.getMinutes()).substr(-2, 2);
        var second_str = ("0" + date.getSeconds()).substr(-2, 2);


        format_str = format;
        format_str = format_str.replace(/YYYY/g, year_str);
        format_str = format_str.replace(/MM/g, month_str);
        format_str = format_str.replace(/DD/g, day_str);
        format_str = format_str.replace(/HH/g, hour_str);
        format_str = format_str.replace(/mm/g, minute_str);
        format_str = format_str.replace(/ss/g, second_str);

        return format_str;
    };

    naite.dateToString = function (date) {
        return naite.dateFormatToString(date, 'YYYY/MM/DD');
    };

    naite.timeToString = function (date) {
        return naite.dateFormatToString(date, 'HH:mm:ss');
    };

    naite.dateTimeToString = function (date) {
        return naite.dateFormatToString(date, 'YYYY-MM-DD HH:mm:ss');
    };

    naite.createLinkButton = function (url, title, clazz, code, name, icon) {
        return '<a href="' + url + '" title="' + title + '" class="btn ' + clazz + ' btn-icon mg-l-5" data-code="' + code + '" data-name="' + name + '"><div class="tx-16"><i class="' + icon + '"></i></div></a>';
    };

    naite.createDialogButton = function (title, clazz, code, name, target, icon) {
        return '<a href="#" title="' + title + '" class="btn ' + clazz + ' btn-icon mg-l-5" data-code="' + code + '" data-name="' + name + '" data-toggle="modal" data-target="' + target + '"><div class="tx-20"><i class="' + icon + '"></i></div></a>';
    };
    naite.createDialogButton2 = function (title, clazz, code1, code2, name, target, icon) {
        return '<a href="#" title="' + title + '" class="btn ' + clazz + ' btn-icon mg-l-5" data-code1="' + code1 + '" data-code2="' + code2 + '" data-name="' + name + '" data-toggle="modal" data-target="' + target + '"><div class="tx-20"><i class="' + icon + '"></i></div></a>';
    };
    naite.createDialogButton3 = function (title, clazz, code1, code2, code3, name, target, icon) {
        return '<a href="#" title="' + title + '" class="btn ' + clazz + ' btn-icon mg-l-5" data-code1="' + code1 + '" data-code2="' + code2 + '" data-code3="' + code3 + '" data-name="' + name + '" data-toggle="modal" data-target="' + target + '"><div class="tx-20"><i class="' + icon + '"></i></div></a>';
    };
    naite.createDialogButton4 = function (title, clazz, code1, code2, code3, code4, name, target, icon) {
        return '<a href="#" title="' + title + '" class="btn ' + clazz + ' btn-icon mg-l-5" data-code1="' + code1 + '" data-code2="' + code2 + '" data-code3="' + code3 + '" data-code4="' + code4 + '" data-name="' + name + '" data-toggle="modal" data-target="' + target + '"><div class="tx-20"><i class="' + icon + '"></i></div></a>';
    };
    naite.createDialogButton8 = function (title, clazz, code1, code2, code3, code4, code5, code6, code7, code8, name, target, icon) {
        return '<a href="#" title="' + title + '" class="btn ' + clazz + ' btn-icon mg-l-5" data-code1="' + code1 + '" data-code2="' + code2 + '" data-code3="' + code3 + '" data-code4="' + code4 + '" data-code5="' + code5 + '" data-code6="' + code6 + '" data-code7="' + code7 + '" data-code8="' + code8 + '" data-name="' + name + '" data-toggle="modal" data-target="' + target + '"><div class="tx-20"><i class="' + icon + '"></i></div></a>';
    };
    naite.createDialogButton9 = function (title, clazz, code1, code2, code3, code4, code5, code6, code7, code8, code9, name, target, icon) {
        return '<a href="#" title="' + title + '" class="btn ' + clazz + ' btn-icon mg-l-5" data-code1="' + code1 + '" data-code2="' + code2 + '" data-code3="' + code3 + '" data-code4="' + code4 + '" data-code5="' + code5 + '" data-code6="' + code6 + '" data-code7="' + code7 + '" data-code8="' + code8 + '" data-code9="' + code9 + '" data-name="' + name + '" data-toggle="modal" data-target="' + target + '"><div class="tx-20"><i class="' + icon + '"></i></div></a>';
    };
    naite.createDialogButton10 = function (title, clazz, code1, code2, code3, code4, code5, code6, code7, code8, code9, code10, name, target, icon) {
        return '<a href="#" title="' + title + '" class="btn ' + clazz + ' btn-icon mg-l-5" data-code1="' + code1 + '" data-code2="' + code2 + '" data-code3="' + code3 + '" data-code4="' + code4 + '" data-code5="' + code5 + '" data-code6="' + code6 + '" data-code7="' + code7 + '" data-code8="' + code8 + '" data-code9="' + code9 + '" data-code10="' + code10 + '" data-name="' + name + '" data-toggle="modal" data-target="' + target + '"><div class="tx-20"><i class="' + icon + '"></i></div></a>';
    };

    naite.validators = {
        custom: {
            'date': function ($el) {
                return false;
            },
            'fixedlength': function ($el) {
                if ($el.val() === undefined) return true;
                var fixedlength = $el.data('fixedlength');
                if ($el.val().length !== fixedlength) {
                    return $el.data('fixedlength-error') || naite.validators.errors.fixedlength;
                }
            },
            'patt': function ($el) {
                if ($el.val() === undefined) return true;
                var regex = new RegExp($el.data('patt'), 'g');
                if (regex.test($el.val()) === false) {
                    return $el.data('patt-error') || naite.validators.errors.patt;
                }
                return false;
            },
            'tel': function ($el) {
                if ($el.val() === undefined) return true;
                //var tel = $el.val().replace(/[━.*‐.*―.*－.*\-.*ー.*\-]/gi, '');
                var tel = $el.val().replace(/-/gi, '');
                if (!tel.match(/^(0[5-9]0[0-9]{8}|0[1-9][1-9][0-9]{7})$/)) {
                    return $el.data('tel-error') || naite.validators.errors.tel;
                }
            }
        },
        errors: {
            fixedlength: 'incorrect length',
            patt: "incorrect format",
            tel: "incorrect tel format",
        }
    };

    naite.initVue = function () {
        // vee-validateの日本語
        Vue.use(VeeValidate, { locale: 'ja' });

        Vue.component('datepicker', {
            props: {
                value: String
            },
            template: '<input type="text" class="form-control fc-datepicker form-control-datepicker" autocomplete="off" placeholder="YYYY/MM/DD" />',
            mounted: function () {
                var vm = this;
                // Init datepicker
                $(this.$el).datepicker({
                    //dateFormat: 'yyyy-MM-dd',
                    showOtherMonths: true,
                    selectOtherMonths: true,
                    onSelect: function (dateText) {
                        console.log(dateText);
                        vm.$emit('input', dateText);
                    }
                }).on("change", function (event) {
                    vm.$emit('input', event.target.value);
                });
            },
            watch: {
                value: function (value) {
                    // update value
                    $(this.$el).val(value);
                }
            },
            destroyed: function () {
                //$(this.$el).off().select2('destroy')
            }
        });

        VeeValidate.Validator.extend('tel', {
            getMessage: field => field + 'の書式（半角数字、半角ハイフン可）が正しくありません',
            validate: function (value) {
                if (value === undefined) return true;
                //var tel = value.replace(/[━.*‐.*―.*－.*\-.*ー.*\-]/gi, '');
                var tel = value.replace(/-/gi, '');
                if (!tel.match(/^(0[5-9]0[0-9]{8}|0[1-9][1-9][0-9]{7})$/)) {
                    return false;
                }
                return true;
            }
        });
        VeeValidate.Validator.extend('zip', {
            getMessage: field => field + 'の書式（半角数字、nnn-nnnn）が正しくありません',
            validate: function (value) {
                if (value === undefined) return true;
                if (!value.match(/^([0-9]{3}|[0-9]{3}-[0-9]{4}|)$/)) {
                    return false;
                }
                return true;
            }
        });

        VeeValidate.Validator.extend('passwd', {
            getMessage: field => field + 'には大文字・小文字・数字・記号を含めてください',
            validate: function (value) {
                if (value === undefined) return true;
                if (!value.match(/^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^\w\s]).*$/)) {
                    return false;
                }
                return true;
            }
        });
        VeeValidate.Validator.extend('zenkaku', {
            getMessage: field => field + 'の書式（全角文字のみ）が正しくありません',
            validate: function (value) {
                if (value === undefined) return true;
                if (!value.match(/^[^\x01-\x7Eｧ-ﾝﾞﾟ\xA1-\xDF]+$/)) {
                    return false;
                }
                return true;
            }
        });
        VeeValidate.Validator.extend('kana', {
            getMessage: field => field + 'の書式（半角文字のみ）が正しくありません',
            validate: function (value) {
                if (value === undefined) return true;
                if (!value.match(/^[0-9a-zA-Zｧ-ﾝﾞﾟ\- ]+$/)) {
                    return false;
                }
                return true;
            }
        });
        VeeValidate.Validator.extend('alpha_num_kigou', {
            getMessage: field => field + 'の書式（半角文字のみ）が正しくありません',
            validate: function (value) {
                if (value === undefined) return true;
                if (!value.match(/^[!-~]+$/)) {
                    return false;
                }
                return true;
            }
        });
    };

    naite.urls = {
        home: "/",
        notFound: "/error/404?msg=指定されたページは見つかりませんでした。",
        login: "/login",
        logout: "/logout",
        logout_login: "/login?msg=" + encodeURIComponent("ログアウトしました"),
        login_timeout: "/login?msg=" + encodeURIComponent("タイムアウトしました"),
        login_unauthorized: "/login?msg=" + encodeURIComponent("認証に失敗しました"),        
        users: "/settings/users",
        userAdd: "/settings/users/new",
        userEdit: function (id) {
            return "/settings/users/" + id;
        },
        itemDataImports: "/itemDataImports",
        itemDataImportEdit: function (id) {
            return "/itemDataImports/" + id;
        },
    };

    naite.apiBaseUrl = 'https://localhost:7038/api';
    //naite.apiBaseUrl = 'http://wakes.nishiyama.tech/api';

    naite.apiUrls = {
        auth: naite.apiBaseUrl + "/v1/auth",
        users: naite.apiBaseUrl + "/v1/users",
        user: function (id) {
            return naite.apiBaseUrl + "/v1/users/" + encodeURIComponent(id);
        },
        userChangePassword: function (id) {
            return naite.apiBaseUrl + "/v1/users/" + encodeURIComponent(id) + "/changepassword";
        },
        itemFields: naite.apiBaseUrl + "/v1/itemFields",
        itemField: function (id) {
            return naite.apiBaseUrl + "/v1/itemFields/" + encodeURIComponent(id);
        },
        sortItemField: function (id) {
            return naite.apiBaseUrl + "/v1/itemFields/" + encodeURIComponent(id) + "/sort";
        },
        searchItemRows: naite.apiBaseUrl + "/v1/itemRows/search",
        itemRows: naite.apiBaseUrl + "/v1/itemRows",
        itemRow: function (id) {
            return naite.apiBaseUrl + "/v1/itemRows/" + encodeURIComponent(id);
        },
        importItemRows: naite.apiBaseUrl + "/v1/itemRows/import",
        items: naite.apiBaseUrl + "/v1/items",
        item: function (id) {
            return naite.apiBaseUrl + "/v1/items/" + encodeURIComponent(id);
        },
        files: naite.apiBaseUrl + "/v1/files",
        checkFiles: naite.apiBaseUrl + "/v1/files/check",
        file: function (id) {
            return naite.apiBaseUrl + "/v1/files/" + encodeURIComponent(id);
        },
        itemDataImports: naite.apiBaseUrl + "/v1/itemDataImports",
        itemDataImport: function (id) {
            return naite.apiBaseUrl + "/v1/itemDataImports/" + encodeURIComponent(id);
        },
        updateItemDataImportFields: function (id) {
            return naite.apiBaseUrl + "/v1/itemDataImports/" + encodeURIComponent(id) + "/fields";
        },
        downloadItemDataImport: function (id) {
            return naite.apiBaseUrl + "/v1/itemDataImports/" + encodeURIComponent(id) + "/download";
        },
        itemDataImportFields: naite.apiBaseUrl + "/v1/itemDataImportFields",
        itemDataImportField: function (id) {
            return naite.apiBaseUrl + "/v1/itemDataImportFields/" + encodeURIComponent(id);
        },
        itemDatas: naite.apiBaseUrl + "/v1/itemDatas",
    };
});