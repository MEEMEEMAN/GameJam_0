using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using Newtonsoft.Json;
using UnityEngine.Networking;
using System;
using System.Diagnostics;
using System.Text;

namespace Perhaps
{
    public class PerhapsRESTTool : UnityEditor.EditorWindow
    {
        [MenuItem("Perhaps/REST Tool")]

        public static void ShowWindow()
        {
            PerhapsRESTTool tool = (PerhapsRESTTool)EditorWindow.GetWindow(typeof(PerhapsRESTTool));
            tool.titleContent.text = "Perhaps API Tool";
            tool.Show();
        }

        private void OnEnable()
        {

        }
       

        RESTOperation selected;
        Vector2 inputScroll;
        Vector2 outputScroll;
        string uri;
        string inputText;
        string outputText;
        private void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.Height(30), GUILayout.Width(position.width));

            uri = EditorGUILayout.TextField("URI: ", uri, GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            selected = (RESTOperation)EditorGUILayout.EnumPopup("REST Operation: ", selected);

            GUI.enabled = !outgoing;
            if (GUILayout.Button("Send"))
            {
                SendRest(selected, uri, inputText);
            }
            if(GUILayout.Button("Clear"))
            {
                outputText = null;
            }
            GUI.enabled = true;

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(position.height), GUILayout.Width(position.width / 2));
            inputScroll = EditorGUILayout.BeginScrollView(inputScroll);
            inputText = EditorGUILayout.TextArea(inputText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(position.height), GUILayout.Width(position.width / 2));
            outputScroll = EditorGUILayout.BeginScrollView(outputScroll);
            EditorGUILayout.TextArea(outputText, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        bool outgoing = false;
        void SendRest(RESTOperation operation, string uriStr, string body = null)
        {
            if (outgoing)
                return;

            PerhapsRestService.SendRESTRequest(operation, uriStr, body, OnComplete);
            outgoing = true;
        }


        void OnComplete(RestResponse response)
        {
            outgoing = false;
            outputText = $"Response code: {response.code}({PerhapsRestService.GetResponseString(response.code)}). received {response.downloadedBytes} bytes. response took {response.completionTime} ms.\n {response.body}";
        }

    }

}
