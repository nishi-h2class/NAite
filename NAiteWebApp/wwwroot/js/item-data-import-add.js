$(function () {

    // メッセージ表示
    naite.checkParam();
    // パラメータをチェックしてvalueにセット
    var paramArray = naite.getParam() || {};

    naite.vuemodel = new Vue({
        el: '#my-app',
        data: {
            importFile: null,
            importFileName: null,
            isHeader: false,
            isConfirm: false,
            fields: [],
            rows: [],
            dispRows: [],
            dataId: null,
            tags: [
                { id: 'code', name: '商品コード' },
                { id: 'date', name: '日付' },
                { id: 'quantity', name: '数量' }
            ],
            fileType: 'Shipping'
        },
        mounted: function () {
            naite.vuemodel = this;
        },
        methods: {
            selectedFile: async function (e) {
                e.preventDefault();
                let files = e.target.files;
                naite.vuemodel.$data.importFile = files[0];
                naite.vuemodel.$data.importFileName = files[0].name;

                swal({
                    title: "ファイルをアップロードしますか？",
                    text: "",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonText: "はい",
                    cancelButtonText: "いいえ"
                }).then(function (isConfirm) {
                    if (isConfirm.value) {
                        // 二重クリック防止
                        naite.loading(true);
                        let formData = new FormData();
                        formData.append('file', files[0]);
                        naite.FileUpload(naite.apiUrls.itemDataImports, formData)
                            .done(function (data) {
                                naite.loading(false);
                                swal({
                                    title: "アップロードが完了しました",
                                    text: "",
                                    type: "success"
                                }).then(function () {
                                    // データの表示
                                    naite.vuemodel.$data.dataId = data.id;
                                    naite.vuemodel.createDisplayData(data.rows);
                                });
                            })
                            .fail(function (error) {
                                console.log(error);
                                var msg = naite.handleError(error);
                                if (msg) {
                                    swal(msg, "", "error");
                                }
                            });

                    } else {
                        swal("アップロードをキャンセルしました", "", "error");
                    }
                });
            },
            createDisplayData: function (rows) {
                console.log(rows);
                this.rows = rows;
                this.fields = [];
                if (rows.length > 0) {
                    for (var i = 0; i < rows[0].length; i++) {
                        var field = {
                            tag: ''
                        };
                        this.fields.push(field);
                    }
                }

                // 行列入替
                var _dispRows = rows.slice();
                if (this.isHeader) {
                    _dispRows.shift();
                }
                else if (this.isHeader == false && rows.length > 5) {
                    _dispRows.pop();
                }

                /* ① 行・列を入れ替える転置関数 */
                const transpose = a => a[0].map((_, c) => a.map(r => r[c]));
                this.dispRows = transpose(_dispRows);
            },
            changeIsHeader: function () {
                if (this.rows.length > 0) {
                    // 行列入替
                    var _dispRows = this.rows.slice();
                    if (this.isHeader) {
                        _dispRows.shift();
                    }
                    else if (this.isHeader == false && this.rows.length > 5) {
                        _dispRows.pop();
                    }
                    /* ① 行・列を入れ替える転置関数 */
                    const transpose = a => a[0].map((_, c) => a.map(r => r[c])); //
                    this.dispRows = transpose(_dispRows);
                }
            },
            putData: function () {
                // パラメータ
                var item = {
                    Id: naite.vuemodel.$data.dataId,
                    FileType: naite.vuemodel.$data.fileType,
                    IsHeader: naite.vuemodel.$data.isHeader,
                    UserId: $.cookie('userId'),
                    Fields: naite.vuemodel.$data.fields
                }
                console.log(item);

                naite.put(naite.apiUrls.itemDataImport(item.Id), item)
                    .done(function (data) {
                        console.log(data);
                        // 二重クリック解除
                        naite.loading(false);
                        swal({
                            title: "登録が完了しました",
                            text: "",
                            type: "success"
                        }).then(function () {
                            naite.redirect(naite.urls.itemDataImports);
                        });
                    })
                    .fail(function (error) {
                        console.log(error);
                        var msg = naite.handleError(error);
                        if (msg) {
                            swal(msg, "", "error");
                        }
                    });
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {

                            if (naite.vuemodel.$data.isConfirm == false) {
                                swal("確認にチェックを入れてください", "", "error");
                                return false;
                            }

                            if (naite.vuemodel.$data.dataId == null) {
                                swal("ファイルをアップロードしてください", "", "error");
                                return false;
                            }

                            // タグが選択されているか、2つ以上選択されていないか
                            var codes = naite.vuemodel.$data.fields.filter(a => a.tag == 'code');
                            if (codes.length == 0) {
                                swal("商品コードを選択してください", "", "error");
                                return false;
                            }
                            if (codes.length > 1) {
                                swal("商品コードは1つだけ選択してください", "", "error");
                                return false;
                            }

                            var dates = naite.vuemodel.$data.fields.filter(a => a.tag == 'date');
                            if (dates.length == 0) {
                                swal("日付を選択してください", "", "error");
                                return false;
                            }
                            if (dates.length > 1) {
                                swal("日付は1つだけ選択してください", "", "error");
                                return false;
                            }

                            var quantities = naite.vuemodel.$data.fields.filter(a => a.tag == 'quantity');
                            if (quantities.length == 0) {
                                swal("数量を選択してください", "", "error");
                                return false;
                            }
                            if (quantities.length > 1) {
                                swal("数量は1つだけ選択してください", "", "error");
                                return false;
                            }

                            swal({
                                title: "登録しますか？",
                                text: "",
                                type: "warning",
                                showCancelButton: true,
                                confirmButtonClass: "btn btn-teal",
                                confirmButtonText: "はい",
                                cancelButtonClass: "btn btn-default",
                                cancelButtonText: "キャンセル"
                            }).then(function (isConfirm) {
                                if (isConfirm.value) {
                                    // 二重クリック防止
                                    naite.loading(true);
                                    naite.vuemodel.putData();

                                } else {
                                    swal.closeModal();
                                }
                            });
                        }
                    });
            }
        }
    });
});