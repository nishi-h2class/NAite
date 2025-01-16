$(function () {

    // メッセージ表示
    naite.checkParam();
    // ページ
    naite.initPagination(getPage);
    // パラメータをチェックしてvalueにセット
    var paramArray = naite.getParam() || {};

    naite.vuemodel = new Vue({
        el: '#my-app',
        data: {
            keyword: paramArray.keyword || null,
            pageSize: paramArray.pageSize || 100,
            orderBy: 'name asc',
            pageNumber: paramArray.pageNumber || 1
        },
        mounted: function () {
            naite.vuemodel = this;
            getPage(paramArray.pageNumber || 1);
        },
        methods: {
            updateFilter: function () {
                getPage(1);
            },
            uploadFile: function () {
                $('#uploadFileModal').modal('show');
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {
                            getPage(1);
                        }
                    });
            }
        }
    });

    function getPage(pageNumber) {
        // 二重クリック防止
        naite.loading(true);

        // パラメータ
        var item = JSON.parse(JSON.stringify(naite.vuemodel.$data));
        item.pageNumber = pageNumber;

        // URLに新しいクエリストリングを付与
        var query = "";
        if (item.keyword) query += "&keyword=" + encodeURIComponent(item.keyword);
        if (item.pageSize) query += "&pageSize=" + encodeURIComponent(item.pageSize);
        if (item.pageNumber) query += "&pageNumber=" + encodeURIComponent(item.pageNumber);
        if (item.orderBy) query += "&orderBy=" + encodeURIComponent(item.orderBy);
        if (query.length > 0) query = "?" + query.substr(1);
        window.history.replaceState(null, null, $(location).attr('pathname') + query);

        naite.get(naite.apiUrls.files, item)
            .done(function (data, status, xhr) {
                console.log(data);
                // ページング作成
                var _pagination = xhr.getResponseHeader('x-pagination');
                var pagination = JSON.parse(_pagination);
                naite.createPagination(pagination.TotalPages, pagination.CurrentPage, 10);

                $('.dataTable tbody').empty();
                for (var i = 0; i < data.length; i++) {
                    $('.dataTable tbody').append(
                        $('<tr></tr>')
                            .append(
                                $('<td></td>').append($('<a target="_blank"></a>').attr('href', data[i].url).text(data[i].name))
                            )
                            .append(
                                $('<td class="text-center" style="width:40px;"></td>')
                                    .append(
                                        $('<a class="text-danger delete" href="#"></a>')
                                            .attr('data-id', data[i].id)
                                            .append(
                                                $('<i class="fa fa-trash" style="font-size:x-large;"></i>')
                                            )
                                    )
                            )
                    );
                }

                naite.loading(false);
            })
            .fail(function (error) {
                console.log(error);
                // naite.handleErrorで、naite.loading(false);
                // をしている
                naite.handleError(error);
            });
    }

    function deleteItem(id) {
        swal({
            title: "ファイルを削除しますか？\r\n削除した場合、商品データベースからも削除されます。",
            text: "",
            type: "warning",
            showCancelButton: true,
            confirmButtonText: "はい",
            cancelButtonText: "いいえ"
        }).then(function (isConfirm) {
            if (isConfirm.value) {
                naite.delete(naite.apiUrls.file(id), null)
                    .done(function (data) {
                        console.log(data);
                        swal({
                            title: "削除が完了しました",
                            text: "",
                            type: "success"
                        }).then(function () {
                            getPage(1);
                        });
                    })
                    .fail(function (error) {
                        console.log(error);
                        swal("削除に失敗しました", "", "error");
                    });
            } else {
                swal("キャンセルしました", "", "error");
            }
        });
    }

    $(document).on("click", "a.delete", function (event) {
        event.preventDefault();
        deleteItem($(this).data('id'));
    });

    // ファイルのアップロード
    $(function () {
        naite.uploadFileVuemodel = new Vue({
            el: '#uploadFileForm',
            data: {
                status: 1,
                uploaded: 0,
                total: 0,
                progress: 0,
                names: []
            },
            mounted: function () {
                naite.uploadFileVuemodel = this;
            },
            watch: {
                uploaded: function (newVal, oldVal) {
                    this.setProgress();
                    if (newVal != 0 && newVal == this.total) {
                        this.status = 4;
                        this.$refs.image.value = null;
                        this.uploaded = 0;
                        this.total = 0;
                        this.progress = 0;
                        setTimeout(() => {
                            $('#uploadFileModal').modal('hide');
                            getPage(1);
                        }, 2000); 
                    }
                }
            },
            methods: {
                cancel: function () {
                    $('#uploadFileModal').modal('hide');
                },
                selectImage: function () {
                    this.$refs.image.click();
                },
                inputImage: function () {
                    this.checkFileName(this.$refs.image.files);
                },
                checkFileName: function (images) {
                    this.status = 2;
                    // 二重クリック防止
                    naite.loading(true);

                    this.names = [];
                    for (i = 0; i < images.length; i++) {
                        this.names.push(images[i].name);
                    }
                    console.log(this.names);

                    naite.post(naite.apiUrls.checkFiles, this.names)
                        .done(function (data) {
                            console.log(data);
                            // 二重クリック解除
                            naite.loading(false);
                            if (data.length > 0) {
                                swal({
                                    title: "既に存在するファイルがあります。\r\n上書きしますか？\r\n\r\n" + data.join('\r\n'),
                                    text: "",
                                    type: "warning",
                                    showCancelButton: true,
                                    confirmButtonText: "はい",
                                    cancelButtonText: "いいえ"
                                }).then(function (isConfirm) {
                                    if (isConfirm.value) {
                                        naite.uploadFileVuemodel.uploadImage(images);
                                    } else {
                                        swal("キャンセルしました", "", "error");
                                    }
                                });
                            }
                            else {
                                naite.uploadFileVuemodel.uploadImage(images);
                            }
                        })
                        .fail(function (error) {
                            console.log(error);
                            var msg = naite.handleError(error);
                            if (msg) {
                                swal(msg, "", "error");
                            }
                        });
                },
                uploadImage: function (images) {
                    console.log('uploadImage');
                    this.status = 3;
                    this.uploaded = 0;
                    this.total = images.length;
                    for (let i = 0; i < images.length; i++) {
                        console.log('1');
                        let formData = new FormData();
                        formData.append('file', images[i]);
                        console.log('2');
                        naite.FileUpload(naite.apiUrls.files, formData)
                            .done(function (data) {
                                console.log(data);
                                naite.uploadFileVuemodel.$data.uploaded++;
                                naite.uploadFileVuemodel.$data.files.push(data);
                            })
                            .fail(function (error) {
                                console.log(error);
                                naite.handleError(error);
                                naite.uploadFileVuemodel.$data.status = 4;
                                naite.uploadFileVuemodel.$refs.image.value = null;
                                naite.uploadFileVuemodel.$data.uploaded = 0;
                                naite.uploadFileVuemodel.$data.total = 0;
                                naite.uploadFileVuemodel.$data.progress = 0;
                                alert('アップロードに失敗したファイルがあります。');
                            });
                    }
                },
                dropImage: function (event) {
                    console.log('dropImage');
                    let images = event.dataTransfer.files;
                    this.checkFileName(images);
                },
                setProgress: function () {
                    if (this.uploaded == 0) {
                        this.progress = 0;
                    } else {
                        this.progress = (this.uploaded / this.total) * 100;
                    }
                    console.log(this.progress);
                },
                resetStatus: function () {
                    this.status = 1;
                }
            }
        });
    });
});