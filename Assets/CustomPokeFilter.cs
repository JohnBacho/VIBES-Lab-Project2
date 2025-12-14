using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

public class CustomPokeFilter : MonoBehaviour, IXRHoverFilter, IXRSelectFilter
{
    [SerializeField]
    private XRPokeFilter pokeFilter;

    void Start()
    {
        if (pokeFilter == null)
            pokeFilter = GetComponent<XRPokeFilter>();

        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            // Add ourselves as filters AFTER the poke filter
            interactable.hoverFilters.Add(this);
            interactable.selectFilters.Add(this);
        }
    }

    void OnDestroy()
    {
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverFilters.Remove(this);
            interactable.selectFilters.Remove(this);
        }
    }

    public bool canProcess => isActiveAndEnabled;

    // Block non-poke interactors from hovering
    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        return interactor is XRPokeInteractor;
    }

    // Block non-poke interactors from selecting
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        return interactor is XRPokeInteractor;
    }
}