using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAllCertificationHandler : UnityEngine.Networking.CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true; //base.ValidateCertificate(certificateData);
    }
}
