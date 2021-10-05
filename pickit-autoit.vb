
;----------------------------------------- D2R Get Picked by Trikk ---------------------------------------

#include  <Misc.au3>
#include  <MsgBoxConstants.au3>
#include  <TrayConstants.au3>
#include  <WinAPIFiles.au3>
#include  <Timers.au3>
#include  "ImageSearch\ImageSearch.au3"
#include  <WinAPI.au3>


;----------------------------------------------------------------------------------------------------------

; Start Options Section

AutoItSetOption ("TrayMenuMode", 0);             ; Show Checkmarks In Tray
AutoItSetOption ("TrayAutoPause", 0);            ; Do Not Auto Pause When Tray Icon Is Clicked
AutoItSetOption ("TrayIconDebug", 1);            ; Show Debug Info In Tray
AutoItSetOption ("WinTextMatchMode", 2);         ; Quick Mode
AutoItSetOption ("WinTitleMatchMode", 3);        ; Exact Title Match
AutoItSetOption ("MouseClickDelay", 5);          ; Number of ms to pause between each mouse click - default 10ms
AutoItSetOption ("MouseClickDownDelay", 1);      ; Number of ms to hold down mouse button after clicking - default 10ms
AutoItSetOption ("SendKeyDelay", 1);             ; Number of ms to pause between each key press - default 5ms
AutoItSetOption ("SendKeyDownDelay", 1);         ; Number of ms to hold down each key after pressing - default 5ms
AutoItSetOption ("PixelCoordMode", 0);           ; Coordinates of a found pixel. [ 0 = window ] [ 1 = screen ] [ 2 = client ]
AutoItSetOption ("MouseCoordMode", 0);           ; Coordinates used for mouse clicks. [ 0 = window ] [ 1 = screen ] [ 2 = client ]
AutoItSetOption("MustDeclareVars", 1);           ;0=no, 1=require pre-declaration

TraySetIcon("GetPicked.ico")
TraySetState($TRAY_ICONSTATE_SHOW)
TraySetToolTip("D2RGetPicked")

TrayTip("D2RGetPicked", "Written By Trikk", 10)

; End Options Section

;----------------------------------------------------------------------------------------------------------

; Start Window Section

Local $handle = WinGetHandle ( "Diablo II: Resurrected" )

Local $window = WinGetPos ( $handle )
Local $xresolution = ($window[2])
Local $yresolution = ($window[3])

; End Window Section

;----------------------------------------------------------------------------------------------------------

; Start Variables Section

Global $Pause
Global $Autoloot
Global $InventoryOpened = False
Global $foundRune = False
Global $foundUnique = False
Global $tradeWindow = False
Global $debugMode = False


Global $Frequency = 50

;Global $ClickCounter
;$ClickCounter = 0
;$ClickCounter += 1

; End Variables Section

;----------------------------------------------------------------------------------------------------------

; Start Hotkeys Section

HotKeySet("{END}", "Terminate")
;HotKeySet("{MOUSE3}", "AutoLeaveGame")
;HotKeySet("{PAUSE}", "Pause")
HotKeySet("{DELETE}", "Autoloot")

; End Hotkeys Section

;----------------------------------------------------------------------------------------------------------

; Start Script Section

SendKeepActive ( $handle )
ToolTip("Hotkeys: {DEL} - Autoloot | {Mouse X1} - Quickleave | {INS} Debug | {END} Terminate Script", 0, 4)

While WinExists ( $handle )

    If WinActive ( $handle ) Then

        If _IsPressed ( "05" ) Then
            Call("AutoLeaveGame")
         EndIf

        If _IsPressed ( "2D" ) Then
            For $x = 1 To 200;
            _WinAPI_DrawRect(($xresolution / 2.7), ($yresolution / 4.2), ($xresolution / 1.55), ($yresolution / 1.45), 0x0000FF)
            Next
        EndIf

    EndIf

WEnd

; End Script Section

;----------------------------------------------------------------------------------------------------------

; Start Core Functions Section

Func Terminate()
    TrayTip("D2RGetPicked", "Terminating Script", 10)
    ToolTip("Later nerd...", 0, 4)
    Exit
EndFunc

Func Pause()
    If NOT $Pause Then
        TrayTip("D2RGetPicked", "Script Paused.", 10)
        ToolTip("D2RGetPicked: AUTOLOOT PAUSED", 0, 4)
    EndIf
    $Pause = NOT $Pause
    While $Pause
        sleep(100)
    WEnd
    TrayTip("D2RGetPicked", "Script Unpaused.", 10)
    ToolTip("D2RGetPicked: Autoloot Enabled", 0, 4)
EndFunc
; End Core Functions Section

;----------------------------------------------------------------------------------------------------------

; Start Loot Functions Section
; CCB980 - unique
; 00FC00 - green set item
; FFAF00 - rune

Func Loot()
    Dim $aiCoord, $asColors[3] = [0xCCB980, 0x00FC00, 0xFFAF00]

    If WinActive ( $handle ) Then

        If _IsPressed ( "49" ) OR _IsPressed ( "42" ) Then
            If NOT $InventoryOpened Then ; if true, set as inventory opened
                ToolTip("D2RGetPicked: AUTOLOOT PAUSED FOR 10 SECONDS", 0, 4)
                sleep(10000)
                $InventoryOpened = False
                ToolTip("D2RGetPicked: Autoloot Enabled", 0, 4)
            Else
                MsgBox($MB_SYSTEMMODAL, "Inventory", "Closed.", 2)
                $InventoryOpened = False ; inventory closed
                ToolTip("D2RGetPicked: Autoloot Enabled", 0, 4)
            EndIf
        EndIf

        If _IsPressed ( "1B" ) Then
            If NOT $InventoryOpened Then
            Else
                MsgBox($MB_SYSTEMMODAL, "Inventory", "Closed.", 2)
                ToolTip("D2RGetPicked: Autoloot Enabled", 0, 4)
                $InventoryOpened = False ; inventory closed
            EndIf
         EndIf


        For $sColor In $asColors
            Local $bCoord = PixelSearch(($xresolution / 2.7), ($yresolution / 4.2), ($xresolution / 1.55), ($yresolution / 1.45), $sColor, 0, 2, $handle)
            If not    @Error Then
                ; Maybe teleport before?
                ;MouseClick($MOUSE_CLICK_RIGHT, ($bCoord[0] + 80), ($bCoord[1] + 40), 2, 1 )

                  If $sColor = "0xFFAF00" Then
                      cr("FOUND RUNE WITH sCOLOR: " & $sColor)
                      ;MouseClick($MOUSE_CLICK_LEFT, ($bCoord[0] + (@DesktopWidth * .03125)), ($bCoord[1] + (@DesktopHeight * .01083)), 2, 1 )
                      ;sleep($Frequency)
                      $foundRune = _ImageSearchArea("Images\rune.png", 1, ($xresolution / 2.7), ($yresolution / 4.2), ($xresolution / 1.55), ($yresolution / 1.45), 100, 0)
                      If IsArray($foundRune) Then
                        cr("_ImageSearchArea: " & $foundRune[0] & "," & $foundRune[1])
                        MouseClick($MOUSE_CLICK_LEFT, ($foundRune[0]), ($foundRune[1]), 2, 1 )
                      EndIf
                  ElseIf $sColor = "0xCCB980" Then
                      cr("FOUND UNIQUE ITEM WITH sCOLOR: " & $sColor)
                      MouseClick($MOUSE_CLICK_LEFT, ($bCoord[0] + (@DesktopWidth * .03125)), ($bCoord[1] + (@DesktopHeight * .02083)), 2, 1 )

                   ElseIf $sColor = "0x00FC00" OR $sColor = "0xFFAF00" Then
                      cr("FOUND GREEN SET ITEM WITH sCOLOR: " & $sColor)
                      MouseClick($MOUSE_CLICK_LEFT, ($bCoord[0] + (@DesktopWidth * .03125)), ($bCoord[1] + (@DesktopHeight * .02083)), 2, 1 )

                  Endif

            EndIf
        Next
    EndIf
EndFunc

Func Autoloot()
    If NOT $Autoloot Then
        TrayTip("D2RGetPicked", "Autoloot Enabled", 10)
        ToolTip("D2RGetPicked: Autoloot Enabled", 0, 4)
    EndIf
    $Autoloot = NOT $Autoloot
    While $Autoloot

        If WinActive ( $handle ) Then

        Call("Loot")

            If _IsPressed ( "05" ) Then
                Call("AutoLeaveGame")
                ExitLoop
            EndIf

            If _IsPressed ( "0D" ) Then
                If NOT $Pause Then
                    TrayTip("D2RGetPicked", "AUTOLOOT PAUSED", 10)
                    ToolTip("D2RGetPicked: AUTOLOOT PAUSED FOR 3 SECONDS", 0, 4)
                EndIf
                $Pause = NOT $Pause
                While $Pause
                    sleep(3000)
                WEnd
                TrayTip("D2RGetPicked", "AUTOLOOT UNPAUSED", 10)
                ToolTip("D2RGetPicked: Autoloot Enabled", 0, 4)
            EndIf


        EndIf

    WEnd
    TrayTip("D2RGetPicked", "Autoloot Disabled", 10)
    ToolTip("D2RGetPicked: Autoloot Disabled", 0, 4)
EndFunc

; End Loot Functions Section

;----------------------------------------------------------------------------------------------------------

; Quality of Life Section

Func AutoLeaveGame()
    If WinActive ( $handle ) Then
            TrayTip("D2RGetPicked", "Leaving Game.", 10)
            Send ("{ESC}")
            sleep($Frequency)
            MouseClick($MOUSE_CLICK_LEFT, ($xresolution / 2), (($yresolution / 2) - ($yresolution * .0694444)), 2, 0 )
            ToolTip("Hotkeys: {DEL} - Autoloot | {Mouse X1} - Quickleave | {INS} Debug | {END} Terminate Script", 0, 4)
    EndIf
 EndFunc


; Debug - show field of view to grab items
Func _WinAPI_DrawRect($start_x, $start_y, $iWidth, $iHeight, $iColor)
    Local $hDC = _WinAPI_GetWindowDC(0) ; DC of entire screen (desktop)
    Local $tRect = DllStructCreate($tagRECT)
    DllStructSetData($tRect, 1, $start_x)
    DllStructSetData($tRect, 2, $start_y)
    DllStructSetData($tRect, 3, $iWidth)
    DllStructSetData($tRect, 4, $iHeight)
    Local $hBrush = _WinAPI_CreateSolidBrush($iColor)

    _WinAPI_FrameRect($hDC, DllStructGetPtr($tRect), $hBrush)
    ; clear resources
    _WinAPI_DeleteObject($hBrush)
    _WinAPI_ReleaseDC(0, $hDC)
EndFunc   ;==>_WinAPI_DrawRect
