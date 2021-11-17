using UnityEngine;
using UnityEngine.UIElements;

[UnityEngine.Scripting.Preserve]
public class Dialog : MonoBehaviour
{
    public VisualElement m_root;
    public VisualTreeAsset dialogOptionUXML;

    private ListView m_dialogOptions;

    private string[] MOCKDIALOG = new string[3] { "I am dialog option 1.", "Go to next option.", "Choose me" };

    private void Start()
    {
        UIDocument doc = GetComponent<UIDocument>();
        if (doc == null)
        {
            Debug.LogError($"{name}: Could not find UIDocument component - Cannot initialize dialog");
            return;
        }
        m_root = doc.rootVisualElement;
        m_dialogOptions = m_root.Q<ListView>("dialog-options");

        m_dialogOptions.makeItem = dialogOptionUXML.CloneTree;
        m_dialogOptions.bindItem = (elem, index) => {
            Character chara = m_characters[index];
            AssetCard card = new AssetCard(
                elem,
                onHovered: null,
                onClicked: () => {
                    ChangeTab(m_completableList);
                    UpdateCompletableList(chara.name);
                },
                onDoubleClicked: null);
            card.Bind(chara);
        };
        m_dialogOptions.itemHeight = 200;
        m_dialogOptions.itemsSource = MOCKDIALOG;

        m_dialogOptions.Refresh();
    }
}